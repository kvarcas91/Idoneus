using Common;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Comments;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class ProjectRepository : BaseRepository
    {

        public IEnumerable<Project> GetProjects()
        {
            var output = GetAll<Project>();
            foreach (var item in output)
            {
                GetProjectContent(item);
                item.GetProgress();
                if (item.DueDate < DateTime.Now && item.Status != Status.Completed)
                {
                    item.Status = Status.Delayed;
                    Task.Run(() => Update(item));
                }
            }
            return output;
        }

        public Project GetProject(string ID)
        {
            var project = Get<Project>("ID", ID, "projects");
            GetProjectContent(project);
            if (project.DueDate < DateTime.Now && project.Status != Status.Completed)
            {
                project.Status = Status.Delayed;
                Task.Run(() => Update(project));
            }
            return project;
        }


        public IEnumerable<Contributor> GetAllContributors()
        {
            return base.GetAll<Contributor>("SELECT * FROM contributors");
        }

        public Contributor GetContributor(string[] names)
        {
            
            var query = $"SELECT * FROM contributors WHERE FirstName = '{names[0]}' AND LastName = '{names[1]}'";
            return Get<Contributor>(query);
        }

        public bool AssignContributor(string projectID, string contributorID)
        {
            var query = $"INSERT INTO project_contributors (projectID, contributorID) VALUES ('{projectID}', '{contributorID}')";
            return Insert(query);
        }

        public bool AssignTaskContributor(string taskID, string contributorID)
        {
            var query = $"INSERT INTO task_contributors (taskID, contributorID) VALUES ('{taskID}', '{contributorID}')";
            return Insert(query);
        }

        private void GetProjectContent(Project project)
        {
            if (project == null) return;
            project.GetProgress();
            project.Tasks = new ObservableCollection<ProjectTask>(GetProjectTasks(project.ID));
            project.Contributors = new ObservableCollection<Contributor>(GetProjectContributors(project.ID));
            project.Comments = new ObservableCollection<IComment>(GetComments(project.ID));
            if (project.Tasks == null) return;

            foreach (var task in project.Tasks)
            {
                task.SubTasks = new ObservableCollection<SubTask>(GetSubTasks(task.ID));
                task.Contributors = new ObservableCollection<Contributor>(GetTaskContributors(task.ID));
                if (task.DueDate < DateTime.Now && task.Status != Status.Completed)
                {
                    task.Status = Status.Delayed;
                    Task.Run(() => Update(task));
                }
                task.GetProgress();
                foreach (var subtask in task.SubTasks)
                {
                    if (subtask.DueDate < DateTime.Now && subtask.Status != Status.Completed)
                    {
                        subtask.Status = Status.Delayed;
                        Task.Run(() => Update(subtask));
                    }
                }
            }
        }

        public IEnumerable<ITask> GetUpcommingTasks(DateTime targetDate)
        {
            var taskQuery = $"SELECT * FROM tasks WHERE Status != '3' AND DueDate <= '{targetDate.ToString(dateFormat)}%' ORDER BY DueDate ASC";
            var subtaskQuery = $"SELECT * FROM subtasks WHERE Status != '3' AND DueDate <= '{targetDate.ToString(dateFormat)}%' ORDER BY DueDate ASC";
            var output = new List<ITask>();
            var tasks = base.GetAll<ProjectTask>(taskQuery);
            var subTasks = base.GetAll<SubTask>(subtaskQuery);
            output.AddRange(tasks);
            output.AddRange(subTasks);
            return output;
        }
        
        public IEnumerable<ProjectTask> GetProjectTasks (string ID)
        {
            var query = $"SELECT * FROM tasks WHERE ParentID = '{ID}'";
            
            return base.GetAll<ProjectTask>(query);
        }

        public IEnumerable<Contributor> GetProjectContributors(string ID)
        {
            var query = QueryHelper.GetComplexQuery(new Contributor(), "contributors", "projects", "project_contributors", ("contributorID", "ID"), ("ID", "projectID"), ID, string.Empty);
            var output = GetAll<Contributor>(query);
            return output;
        }

        public IEnumerable<IComment> GetComments(string ID)
        {
            var commentsQuery = $"SELECT * FROM comments WHERE ProjectID = '{ID}'";
            var linksQuery = $"SELECT * FROM links WHERE ProjectID = '{ID}'";
            var output = new List<IComment>();
            output.AddRange(GetAll<Comment>(commentsQuery));
            output.AddRange(GetAll<Link>(linksQuery));

            return output;
        }

        public IEnumerable<Contributor> GetTaskContributors(string ID)
        {
            var query = QueryHelper.GetComplexQuery(new Contributor(), "contributors", "tasks", "task_contributors", ("contributorID", "ID"), ("ID", "taskID"), ID, string.Empty);
            var output = GetAll<Contributor>(query);
            return output;
        }

        public IEnumerable<SubTask> GetSubTasks (string ID)
        {
            var query = $"SELECT * FROM subtasks WHERE ParentID = '{ID}'";
            return base.GetAll<SubTask>(query);
        }

        public IEnumerable<Project> SortByViewType(IEnumerable<Project> list, string viewType)
        {
            Enum.TryParse(viewType.Replace(" ", string.Empty), out ViewType type);
            return type switch
            {
                ViewType.Completed => list.Where(i => i.Status.Equals(Status.Completed)),
                ViewType.InProgress => list.Where(i => i.Status.Equals(Status.InProgress)),
                ViewType.Archived => list.Where(i => i.Status.Equals(Status.Archived)),
                ViewType.Delayed => list.Where(i => i.Status.Equals(Status.Delayed)),
                _ => list,
            };
        }

        public IEnumerable<ITask> SortByViewType(IEnumerable<ITask> list, string viewType)
        {
            Enum.TryParse(viewType.Replace(" ", string.Empty), out ViewType type);
            return type switch
            {
                ViewType.Completed => list.Where(i => i.Status.Equals(Status.Completed)),
                ViewType.InProgress => list.Where(i => i.Status.Equals(Status.InProgress)),
                ViewType.Archived => list.Where(i => i.Status.Equals(Status.Archived)),
                ViewType.Delayed => list.Where(i => i.Status.Equals(Status.Delayed)),
                _ => list,
            };
        }

        public Project GetParentProject(ITask task)
        {
            if (task is ProjectTask)
            {
                return GetProject(task.ParentID);
            }
            if (task is SubTask)
            {
                var query = $"SELECT ParentID FROM tasks WHERE ID = (SELECT ParentID FROM subtasks WHERE ID = '{task.ID}')";
                string parentID = GetScalar<string>(query);
                return GetProject(parentID);
            }

            return null;
        }

        public void UnassignContributors(IEnumerable<Contributor> contributors, string projectID)
        {
            foreach (var item in contributors)
            {
                UnassignContributor(item.ID, projectID);
            }
        }

        public void UnassignContributor(string contributorID, string projectID)
        {
            var query = $"DELETE FROM project_contributors WHERE projectID = '{projectID}' AND contributorID = '{contributorID}'";
            Delete(query);
        }

        public void UnassignTaskContributors(IEnumerable<Contributor> contributors, string taskID)
        {
            foreach (var item in contributors)
            {
                UnassignTaskContributor(item.ID, taskID);
            }
        }

        public void UnassignTaskContributor(string contributorID, string taskID)
        {
            var query = $"DELETE FROM task_contributors WHERE taskID = '{taskID}' AND contributorID = '{contributorID}'";
            Delete(query);
        }

        public Response DeleteComment(IComment data)
        {
            var table = string.Empty;
            if (data is Comment)
            {
                table = "comments";
            }
            if (data is Link)
            {
                table = "links";
            }

            var query = $"DELETE FROM {table} WHERE ID = '{data.ID}'";
            return new Response { Success = Delete(query) };

        }

        public Response DeleteProject(Project project)
        {

            GetProjectContent(project);

            foreach (var comment in project.Comments)
            {
                DeleteComment(comment);
            }

            UnassignContributors(project.Contributors, project.ID);

            DeleteTasks(project.Tasks);
            Task.Run(() => DeleteProjectData(project.ID));

            Delete(project);
            return new Response();
        }

        public bool DeleteProjectData(string ID)
        {
            var dir = new DirectoryInfo(FileHelper.GetFullPath(Path.Combine(".\\Projects", ID)));
            try
            {
                dir.Delete(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTasks(IEnumerable<ITask> projectTasks)
        {
            foreach (var item in projectTasks)
            {
                if (item is ProjectTask task)
                {
                    UnassignTaskContributors(task.Contributors, task.ID);
                    DeleteTasks(task.SubTasks);
                }
               
                DeleteTask(item);
            }
            return true;
        }

        public bool DeleteTask(ITask task)
        {
            if (task is ProjectTask pTask) Delete(pTask);
            if (task is SubTask sTask) Delete(sTask);
            return true;
        }
    }
}
