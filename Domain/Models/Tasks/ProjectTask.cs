using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;
using System.Collections.ObjectModel;

namespace Domain.Models.Tasks
{

    [Table("tasks")]
    public class ProjectTask : IEntity, IUpdatableProgress, ITask
    {

        [Key]
        public int ID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int OrderNumber { get; set; }

        [Computed]
        public ObservableCollection<SubTask> SubTasks { get; set; } = new ObservableCollection<SubTask>();

        private double _progress = 0D;

        public double GetProgress()
        {
            _progress = 0D;
            if (SubTasks.Count == 0) return _progress;

            double itemWeight = 100;

            foreach (var item in SubTasks)
            {
                if (item.IsCompleted) _progress += itemWeight;
            }

            return _progress;
        }
    }
}
