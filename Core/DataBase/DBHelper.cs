using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Core.DataBase
{
    public class DBHelper
	{

		private static readonly object _lock = new object();
		private static readonly string dateFormat = "yyyy-MM-dd";

		#region Projects

		public static IList<IProject> GetProjects(ViewType viewType)
		{
			
			var sql = "select * from projects";
			switch (viewType)
			{
				case ViewType.Ongoing:
					sql = $"{sql} where IsArchived = '0'";
					break;
				case ViewType.Archived:
					sql = $"{sql} where IsArchived = '1'";
					break;
			}
			sql = $"{sql} order by OrderNumber";


			using IDbConnection connection = new SQLiteConnection(GetConnectionString());

			List<Project> output = connection.Query<Project>(sql).ToList();

			foreach (var project in output)
			{
				project.Tasks = GetProjectTasks(project.ID);
				project.Comments = GetProjectComments(project.ID);
				project.UpdateProgress();
				//project.Progress = GetProjectProgress(project.ID);
				project.AddPersons(GetProjectContributors(project.ID));

			}
			connection.Dispose();

			return output.ConvertAll(o => (IProject)o);
		}

		public static int GetPublishedProjectsCount ()
		{
			return GetCount("projects");
		}

		public static void InsertProject(Project project)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var id = connection.Insert(project);
			project.ID = id;
			
			connection.Dispose();
		}

		public static void UpdateProject(Project project)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Update(project);
			
			connection.Dispose();
		}

		public static void DeleteProject(Project project)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			
			// Removing all tasks
			foreach (var task in project.Tasks)
			{
				DeleteTask(task);
			}

			// Removing all comments
			foreach (var comment in project.Comments)
			{
				DeleteComment(comment);
			}

			// Reassign contributors
			foreach (var contributor in project.Contributors)
			{
				ReAssignContributor(project.ID, contributor.ID);
			}

			connection.Delete(project);
			FileHelper.DeleteFolder(project.Path);
		}

		#endregion // Projects

		#region Tasks

		public static void InsertTask (ITask task, long projectID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var sql = @"insert into tasks (Content, DueDate, Priority, IsCompleted) values (@Content, @DueDate, @Priority, @IsCompleted)";
			connection.Execute(sql, task);
			task.ID = GetLastRowID("tasks");

			connection.Dispose();

			AssignTaskToTheProject(projectID, task.ID);

		}

		private static void AssignTaskToTheProject(long projectID, long taskID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Execute("INSERT INTO project_tasks (projectID, taskID) values (@projectID, @taskID)",
				new { projectID, taskID });
			connection.Dispose();
		}

		public static ObservableCollection<IElement> GetProjectTasks(long projectID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var output = connection.Query<Task>(
				"SELECT t.ID, t.Content, t.IsCompleted, t.Priority, t.DueDate, t.OrderNumber " +
				"FROM tasks t " +
				"INNER JOIN project_tasks p ON p.taskID = t.ID " +
				$"INNER JOIN projects pr on pr.ID = p.projectID WHERE pr.ID = {projectID} order by t.OrderNumber ASC, t.DueDate ASC");
			foreach (var task in output)
			{
				
				task.AddElements(GetSubTasks(task.ID));
				task.AddPersons(GetTaskContributors(task.ID));
                task.UpdateProgress();
                //task.Progress = GetTaskProgress(task.ID);
            }
			connection.Dispose();
			return new ObservableCollection<IElement>(output);
		}

		/// <summary>
		/// Returns tasks count based on a given parameter
		/// </summary>
		/// <param name="isCompleted">If the tasks are completed</param>
		/// <returns></returns>
		public static int GetAllTasks (bool isCompleted)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var count = GetCount("tasks", "IsCompleted", (Convert.ToInt32(isCompleted)).ToString());

			connection.Dispose();
			return count;
		}

		public static int GetOverdueTasks()
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var count = GetCount("tasks", "DueDate", "<", DateTime.Now.ToString("yyyy-MM-dd"));

			connection.Dispose();
			return count;
		}

		public static void UpdateTask (IElement param)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            if (param is SubTask subTask)
                connection.Update(subTask);
            if (param is Task task)
                connection.Update(task);
            connection.Dispose();
		}

        public static void DeleteTask(IElement element)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());

            var task = (ITask)element;

            foreach (var subtask in task.SubTasks)
            {
                string subtaskAssignQuery = $"DELETE FROM task_subtasks WHERE subtaskID = '{subtask.ID}'";
                connection.Execute(subtaskAssignQuery);

                string subtaskQuery = $"DELETE FROM subtasks WHERE ID = '{subtask.ID}'";
                connection.Execute(subtaskQuery);
            }

            string taskQuery = $"DELETE FROM project_tasks WHERE taskID = '{task.ID}'";
            string contrQuery = $"DELETE FROM task_contributors WHERE taskID = '{task.ID}'";

            connection.Execute(taskQuery);
            connection.Execute(contrQuery);

            connection.Delete((Task)task);
            connection.Dispose();
        }

       

        #endregion // Tasks

        #region SubTasks

        private static IList<IElement> GetSubTasks(long taskID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			List<SubTask> tasks = connection.Query<SubTask>(
				"SELECT s.ID, s.Content, s.IsCompleted, s.Priority, s.DueDate, s.OrderNumber " +
				"FROM subtasks s " +
				"INNER JOIN task_subtasks p ON p.subtaskID = s.ID " +
				$"INNER JOIN tasks ts on ts.ID = p.taskID WHERE ts.ID = {taskID} order by s.IsCompleted, s.DueDate, s.Priority DESC").ToList();
			connection.Dispose();
			var output = new List<IElement>();
			foreach (var task in tasks)
			{
				output.Add(task);
			}
			return output;
		}

		public static void InsertSubTask(ISubTask subTask, long taskID)
		{
			using (IDbConnection connection = new SQLiteConnection(GetConnectionString()))
			{
				var order = (uint)GetCount("subtasks");
				subTask.OrderNumber = order;

				var sql = @"insert into subtasks (Content, Priority, IsCompleted, DueDate) 
							values (@Content, @Priority, @IsCompleted, @DueDate)";
				connection.Execute(sql,
								new
								{
									subTask.Content,
									subTask.Priority,
									subTask.IsCompleted,
									subTask.DueDate
								});

				subTask.ID = GetLastRowID("subtasks");
				connection.Dispose();

			}
			AssignSubTaskToTheTask(taskID, GetLastRowID("subtasks"));
		}

		private static void AssignSubTaskToTheTask(long taskID, long subtaskID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Execute("INSERT INTO task_subtasks (taskID, subtaskID) values (@taskID, @subtaskID)",
				new { taskID, subtaskID });
			connection.Dispose();
		}

        public static void DeleteSubTask(ISubTask task)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            connection.Delete((SubTask)task);
            connection.Dispose();
        }

        #endregion

        #region Today's Tasks


        public static ObservableCollection<TodaysTask> GetTodaysTasks(DateTime date)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var query = $"SELECT * FROM td_tasks WHERE SubmitionDate like '{date.ToString(dateFormat)}%' ORDER BY IsCompleted";
			List<TodaysTask> output = connection.Query<TodaysTask>(query).ToList();

			connection.Dispose();

			return new ObservableCollection<TodaysTask>(output);

		}

		public static void InsertTodaysTask (TodaysTask task)
		{

			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var id = connection.Insert(task);
			task.ID = id;
			connection.Dispose();
		}

		public static void UpdateTodaysTask (TodaysTask task)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Update(task);
			connection.Dispose();
		}

		public static void DeleteTodaysTask (TodaysTask task)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Delete(task);
			connection.Dispose();
		}

		public static void DeleteAllTodaysTask (IList<TodaysTask> tasks)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			foreach (var task in tasks)
			{
				connection.Delete(task);
			}
			connection.Dispose();
		}

		#endregion // Today's Tasks

		#region Contributors

		public static ObservableCollection<IContributor> GetAllContributors()
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			List<Contributor> contributors = connection.Query<Contributor>(
				"SELECT * FROM contributors").ToList();
			connection.Dispose();
			var output = new ObservableCollection<IContributor>();
			foreach (var contributor in contributors)
			{
				output.Add(contributor);
			}
			return output;
		}

		public static IList<IPerson> GetProjectContributors(long projectID)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            List<Contributor> contributors = connection.Query<Contributor>(
                "SELECT c.ID, c.FirstName, c.LastName " +
                "FROM contributors c " +
                "INNER JOIN project_contributors p ON p.contributorID = c.ID " +
                $"INNER JOIN projects pr on pr.ID = p.projectID WHERE pr.ID = {projectID}").ToList();
            connection.Dispose();
            var output = new List<IPerson>();
            foreach (var contributor in contributors)
            {
                output.Add(contributor);
            }
            return output;
        }

        public static IList<IPerson> GetTaskContributors(long taskID)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            List<Contributor> contributors = connection.Query<Contributor>(
                "SELECT c.ID, c.FirstName, c.LastName " +
                "FROM contributors c " +
                "INNER JOIN task_contributors p ON p.contributorID = c.ID " +
                $"INNER JOIN tasks pr on pr.ID = p.taskID WHERE pr.ID = {taskID}").ToList();
            connection.Dispose();
            var output = new List<IPerson>();
            foreach (var contributor in contributors)
            {
                output.Add(contributor);
            }
            return output;
        }

		public static void UpdateContributor(IPerson param)
		{
			
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
				connection.Update(param);
			
			connection.Dispose();
		}

		public static void AssignContributors(long pID, long cID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Execute("INSERT INTO project_contributors (projectID, contributorID) values (@projectID, @contributorID)",
				new { projectID = pID, contributorID = cID });
			connection.Dispose();
		}

		public static void ReAssignContributor(long pID, long cID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			string query = $"DELETE FROM project_contributors WHERE projectID = '{pID}' AND contributorID = '{cID}'";
			connection.Execute(query);
			connection.Dispose();
		}

		public static void InsertContributor(Contributor contributor)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var id = connection.Insert(contributor);
			contributor.ID = id;

			connection.Dispose();
		}

        #endregion // Contributors

        #region Comments

		public static void InsertComment (Comment comment, long projectID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var id = connection.Insert(comment);
			comment.ID = id;
			connection.Dispose();

			AssignComment(projectID, id);
		}

		private static void AssignComment (long prID, long cID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Execute("INSERT INTO project_comments (projectID, commentID) values (@prID, @cID)",
				new { prID, cID });
			connection.Dispose();
		}

		public static ObservableCollection<IElement> GetProjectComments(long projectID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var output = connection.Query<Comment>(
				"SELECT c.ID, c.Content, c.SubmitionDate " +
				"FROM comments c " +
				"INNER JOIN project_comments p ON p.commentID = c.ID " +
				$"INNER JOIN projects pr on pr.ID = p.projectID WHERE pr.ID = {projectID} order by c.SubmitionDate DESC");

			connection.Dispose();
			return new ObservableCollection<IElement>(output);
		}

		public static void UpdateComment (IComment comment)
		{
			comment.SubmitionDate = DateTime.Now;
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			connection.Update((Comment)comment);
			connection.Dispose();
		}

		public static void DeleteComment (IElement comment)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			string query = $"DELETE FROM project_comments WHERE commentID = '{comment.ID}'";
			connection.Execute(query);
			connection.Delete((Comment)comment);
		}

		#endregion // Comments

		#region Support DB Methods

		//private static double GetProjectprogress(long projectID)
		//{
		//    var ProjectTasks = GetProjectTasks(projectID);
		//    var totalTaskCount = ProjectTasks.Count;
		//    if (totalTaskCount == 0) return 0;
		//    var completedTaskCount = 0;
		//    foreach (var task in ProjectTasks)
		//    {
		//        if (task.IsCompleted)
		//        {
		//            completedTaskCount++;
		//        }
		//    }
		//    return (completedTaskCount * 100) / totalTaskCount;
		//}

		#endregion // Support DB Methods

		#region Support Methodds

		/// <summary>
		/// Creates tables if database is not found
		/// </summary>
		public static void CreateTablesIfNotExist()
		{
			lock(_lock)
			{
				using IDbConnection connection = new SQLiteConnection(GetConnectionString());

				var query = @"CREATE TABLE projects (
								ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								SubmitionDate TEXT NOT NULL,
								Content TEXT NOT NULL,
								Path TEXT NOT NULL,
								DueDate TEXT NOT NULL,
								Header TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								IsArchived INTEGER,
								OrderNumber INTEGER);

							CREATE TABLE comments (
								ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								Content TEXT NOT NULL,
								SubmitionDate TEXT NOT NULL);

							CREATE TABLE project_comments (
								projectID INTEGER NOT NULL,
								commentID INTEGER NOT NULL,
								FOREIGN KEY(projectID) REFERENCES projects(ID),
								FOREIGN KEY(commentID) REFERENCES comments(ID));

							CREATE TABLE tasks (
								ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								Content TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								DueDate TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								OrderNumber INTEGER);

							CREATE TABLE td_tasks (
								ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								Content TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								SubmitionDate TEXT NOT NULL);

                            CREATE TABLE contributors ( 
                              [ID] INTEGER NOT NULL,
                              [FirstName] TEXT NOT NULL,
                              [LastName] TEXT NOT NULL,
                              CONSTRAINT[PK_Contributor] PRIMARY KEY([ID]));

                            CREATE TABLE project_contributors (
                                projectID INTEGER NOT NULL,
	                            contributorID INTEGER NOT NULL,
	                            FOREIGN KEY(contributorID) REFERENCES contributors(ID),
	                            FOREIGN KEY(projectID) REFERENCES projects(ID));

							CREATE TABLE project_tasks (
								projectID INTEGER NOT NULL,
								taskID   INTEGER NOT NULL,
								FOREIGN KEY(projectID) REFERENCES projects(ID),
								FOREIGN KEY(taskID) REFERENCES tasks(ID));

							CREATE TABLE subtasks(
								ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
								Content TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								DueDate TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								OrderNumber INTEGER);

							CREATE TABLE task_subtasks(
								taskID INTEGER NOT NULL,
								subtaskID INTEGER NOT NULL,
								FOREIGN KEY(subtaskID) REFERENCES subtasks(ID),
								FOREIGN KEY(taskID) REFERENCES tasks(ID));

                            CREATE TABLE task_contributors(
                                taskID INTEGER NOT NULL,
	                            contributorID INTEGER NOT NULL,
	                            FOREIGN KEY(taskID) REFERENCES tasks(ID),
	                            FOREIGN KEY(contributorID) REFERENCES contributors(ID));";




                connection.Execute(query);
			}
			
		}

		private static int GetLastRowID(string table)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var count = connection.ExecuteScalar<int>($"select seq from sqlite_sequence where name='{table}'");
			//var count = connection.Query("SELECT COUNT(*) FROM projects;");
			connection.Dispose();
			return count;
		}

		public static int GetCount(string table)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {table}");
			//var count = connection.Query("SELECT COUNT(*) FROM projects;");
			connection.Dispose();
			return count;
		}

		public static int GetCount(string table, string columnName, string param)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var count = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {table} WHERE {columnName} = '{param}'");
			//var count = connection.Query("SELECT COUNT(*) FROM projects;");
			connection.Dispose();
			return count;
		}

		public static int GetCount(string table, string columnName, string op, string param)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			var sql = $"SELECT COUNT(*) FROM {table} WHERE {columnName} {op} '{param}'";
			var count = connection.ExecuteScalar<int>(sql);
			//var count = connection.Query("SELECT COUNT(*) FROM projects;");
			connection.Dispose();
			return count;
		}

		private static string GetConnectionString(string id = "Default")
		{
			return ConfigurationManager.ConnectionStrings[id].ConnectionString;
		}

		#endregion // Support Methods
	}
}
