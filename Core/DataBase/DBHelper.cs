using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using Dapper;
using Core.DataModels;
using Core.Utils;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Dapper.Contrib.Extensions;

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
				project.UpdateProgress();
				//project.Progress = GetProjectProgress(project.ID);
				//project.AddPersons(GetProjectContributors(project.ID));

			}
			connection.Dispose();

			return output.ConvertAll(o => (IProject)o);
		}

		public static int GetPublishedProjectsCount ()
		{
			return GetCount("projects");
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
				$"INNER JOIN projects pr on pr.ID = p.projectID WHERE pr.ID = {projectID} order by t.OrderNumber");
			foreach (var task in output)
			{
				task.UpdateProgress();
				task.AddElements(GetSubTasks(task.ID));
				//task.AddPersons(GetTaskContributors(task.ID));
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

		#endregion // Tasks

		#region SubTasks

		private static IList<IElement> GetSubTasks(long taskID)
		{
			using IDbConnection connection = new SQLiteConnection(GetConnectionString());
			List<SubTask> tasks = connection.Query<SubTask>(
				"SELECT s.ID, s.Content, s.IsCompleted, s.Priority, s.DueDate, s.OrderNumber " +
				"FROM subtasks s " +
				"INNER JOIN task_subtasks p ON p.subtaskID = s.ID " +
				$"INNER JOIN tasks ts on ts.ID = p.taskID WHERE ts.ID = {taskID} order by s.OrderNumber").ToList();
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
			//var sql = @"insert into td_tasks (Content, SubmitionDate, IsCompleted) values (@Content, @SubmitionDate, @IsCompleted)";
			//connection.Execute(sql, task);
			//task.ID = GetLastRowID("td_tasks");

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
								FOREIGN KEY(taskID) REFERENCES tasks(ID));";



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
