using Common;
using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Models.Base;
using System;
using System.Collections.ObjectModel;

namespace Domain.Models.Tasks
{

    [Table("tasks")]
    [Serializable]
    public class ProjectTask : IEntity, IUpdatableProgress, ITask
    {

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; } = Status.Default;
        public int OrderNumber { get; set; }

        [Computed]
        public ObservableCollection<SubTask> SubTasks { get; set; } = new ObservableCollection<SubTask>();

        [Exportable]
        public double Progress { get; set; }

        public double GetProgress()
        {
            Progress = 0D;
            if (SubTasks.Count == 0) return Progress;

            double itemWeight = 100;

            foreach (var item in SubTasks)
            {
                if (item.Status == Status.Completed) Progress += itemWeight;
            }

            return Progress;
        }

        public bool HasString(string param)
        {

            if (Content.ToLower().Contains(param) ||
                Status.ToString().ToLower().Contains(param)) return true;

            foreach (var subTask in SubTasks)
            {
                if (subTask.HasString(param)) return true;
            }

            return false;
        }
    }
}
