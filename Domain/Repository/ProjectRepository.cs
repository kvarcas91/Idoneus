using Common;
using Domain.Helpers;
using Domain.Models.Base;
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
            var output = base.GetAll<Project>();
            foreach (var item in output)
            {
                item.Tasks = new ObservableCollection<ProjectTask>(GetProjectTasks(item.ID));
                if (item.Tasks == null) continue;

                foreach (var task in item.Tasks)
                {
                    task.SubTasks = new ObservableCollection<SubTask>(GetSubTasks(task.ParentID));
                }
            }
            return output;
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
        
        public dynamic GetParentID<T>(int ID, T obj, string table, string joinTable, string middleTable, (string, string) p1, (string, string) p2) where T : class, IEntity
        {
            var query = QueryHelper.GetComplexQuery(obj, table, joinTable, middleTable, p1, p2, ID, null, singleKeyValue: true);
            return query;
        }

        public IEnumerable<ProjectTask> GetProjectTasks (string ID)
        {
            var query = $"SELECT * FROM tasks WHERE ParentID = '{ID}'";
            
            return base.GetAll<ProjectTask>(query);
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

       

    }
}
