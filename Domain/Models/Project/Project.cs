using Common;
using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Models.Base;
using Domain.Models.Tasks;
using System;
using System.Collections.ObjectModel;

namespace Domain.Models.Project
{
    [Table("projects")]
    [Serializable]
    public class Project : IEntity, IUpdatableProgress
    {

        [Key]
        public string ID { get; set; }

        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Default;
        public Status Status { get; set; } = Status.Default;
        public int OrderNumber { get; set; }
        public double Progress { get; set; } = 0D;

        [Computed]
        public ObservableCollection<ProjectTask> Tasks { get; set; } = new ObservableCollection<ProjectTask>();

        [Computed]
        public int CompletedTaskCount { get; set; } = 0;

        public double GetProgress()
        {
            Progress = 0D;
            if (Status == Status.Completed)
            {
                Progress = 100D;
            }

            if (Tasks.Count == 0) return Progress;

            double itemWeight = 100 / Tasks.Count;
            CompletedTaskCount = 0;

            foreach (var item in Tasks)
            {

                if (item.Status == Status.Completed)
                {
                    Progress += itemWeight;
                    CompletedTaskCount++;
                    continue;
                }

                if (item.SubTasks.Count == 0) continue;

                double itemSubWeight = itemWeight / item.SubTasks.Count;

                Progress += itemSubWeight / 100 * item.GetProgress(); 
            }

            if (CompletedTaskCount == Tasks.Count) Progress = 100D;
           



            return Math.Round(Progress, 0);
        }

        public bool HasString(string param, ViewType viewType)
        {
            var type = viewType switch
            {
                ViewType.All => (int)Status,
                ViewType.Archived => (int)Status.Default,
                _ => (int)viewType,
            };

            var lowerParam = param.ToLower();
            if ((Header.ToLower().Contains(lowerParam) &&  (int)Status == type) ||
                (Content.ToLower().Contains(lowerParam) && (int)Status == type) ||
                (Priority.ToString().ToLower().Contains(lowerParam) && (int)Status == type) ||
                (Status.ToString().ToLower().Contains(lowerParam) && (int)Status == type)) return true;

            foreach (var task in Tasks)
            {
                if (task.HasString(lowerParam) && (int)Status == type) return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID: {ID}; Header: {Header}; Content: {Content}; Submition Date: {SubmitionDate.ToString()}; DueDate: {DueDate.ToString()}";
        }
    }
}
