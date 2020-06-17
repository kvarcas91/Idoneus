using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using Domain.Models.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Models.Project
{
    [Table("projects")]
    public class Project : IEntity, IUpdatableProgress
    {
        [Key]
        public int ID { get; set; }
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
        public Priority Priority { get; set; }
        public bool IsArchived { get; set; }
        public int OrderNumber { get; set; }

        [Computed]
        public ObservableCollection<ProjectTask> Tasks { get; set; } = new ObservableCollection<ProjectTask>();

        [Computed]
        public int CompletedTaskCount { get; set; } = 0;

        public double Progress { get; set; } = 0D;

        public double GetProgress()
        {
            Progress = 0D;
            if (Tasks.Count == 0) return Progress;

            double itemWeight = 100 / Tasks.Count;
            CompletedTaskCount = 0;

            foreach (var item in Tasks)
            {

                if (item.IsCompleted)
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

        public override string ToString()
        {
            return $"ID: {ID}; Header: {Header}; Content: {Content}; Submition Date: {SubmitionDate.ToString()}; DueDate: {DueDate.ToString()}";
        }
    }
}
