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
using System.Linq;

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
            }
            return output;
        }

        public Project GetProject(string ID)
        {
            var project = Get<Project>("ID", ID, "projects");
            GetProjectContent(project);
            return project;
        }

        private void GetProjectContent(Project project)
        {
            project.GetProgress();
            project.Tasks = new ObservableCollection<ProjectTask>(GetProjectTasks(project.ID));
            project.Contributors = new ObservableCollection<Contributor>(GetProjectContributors(project.ID));
            project.Comments = new ObservableCollection<IComment>(GetComments(project.ID));
            if (project.Tasks == null) return;

            foreach (var task in project.Tasks)
            {
                task.SubTasks = new ObservableCollection<SubTask>(GetSubTasks(task.ParentID));
                task.Contributors = new ObservableCollection<Contributor>(GetTaskContributors(task.ID));
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
                ViewType.Archived => list.Where(i => i.Status.Equals(Status.Default)),
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
                ViewType.Archived => list.Where(i => i.Status.Equals(Status.Default)),
                ViewType.Delayed => list.Where(i => i.Status.Equals(Status.Delayed)),
                _ => list,
            };
        }

        public Project GetParentProject(ITask task)
        {
            if (task is ProjectTask)
            {
                return Get<Project>("ID", task.ParentID, "projects");
            }
            if (task is SubTask)
            {
                var query = $"SELECT ParentID FROM subtasks WHERE ID ='{task.ID}'";
                string parentID = GetScalar<string>(query);
                return Get<Project>("ID", parentID, "projects");
            }

            return null;
        }

    }
}
