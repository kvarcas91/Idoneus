using Domain.Helpers;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Repository
{
    public class ProjectRepository : BaseRepository
    {

        private static readonly string dateFormat = "yyyy-MM-dd";

        public IEnumerable<Project> GetProjects()
        {
            var output = base.GetAll<Project>();
            foreach (var item in output)
            {
                item.Tasks = new ObservableCollection<ProjectTask>(GetProjectTasks(item.ID));
                if (item.Tasks == null) continue;

                foreach (var task in item.Tasks)
                {
                    task.SubTasks = new ObservableCollection<SubTask>(GetSubTasks(task.ID));
                }
            }
            return output;
        }

        public IEnumerable<ITask> GetUpcommingTasks(DateTime targetDate)
        {
            var taskQuery = $"SELECT * FROM tasks WHERE IsCompleted= '0' AND DueDate <= '{targetDate.ToString(dateFormat)}%' ORDER BY DueDate ASC";
            var subtaskQuery = $"SELECT * FROM subtasks WHERE IsCompleted= '0' AND DueDate <= '{targetDate.ToString(dateFormat)}%' ORDER BY DueDate ASC";
            var output = new List<ITask>();
            var tasks = base.GetAll<ProjectTask>(taskQuery);
            var subTasks = base.GetAll<SubTask>(subtaskQuery);
            output.AddRange(tasks);
            output.AddRange(subTasks);
            return output;
        }

        public IEnumerable<ProjectTask> GetProjectTasks (int ID)
        {
            var query = QueryHelper.GetComplexQuery(new ProjectTask(), base.GetTableName<ProjectTask>(), "projects", "project_tasks", ("taskID", "ID"), ("ID", "projectID"), ID, "ASC", "OrderNumber", "DueDate");
            
            return base.GetAll<ProjectTask>(query);
        }

        public IEnumerable<SubTask> GetSubTasks (int ID)
        {
            var query = QueryHelper.GetComplexQuery(new SubTask(), base.GetTableName<SubTask>(), "tasks", "task_subtasks", ("subtaskID", "ID"), ("ID", "taskID"), ID, "DESC", "IsCompleted", "DueDate", "Priority");
            return base.GetAll<SubTask>(query);
        }

    }
}
