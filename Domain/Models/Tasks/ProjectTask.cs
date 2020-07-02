using Common;
using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Models.Base;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Domain.Models.Tasks
{

    [Table("tasks")]
    [Serializable]
    public class ProjectTask : BindableBase, IUpdatableProgress, ITask
    {

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }

        private Status _status = Status.InProgress;
        public Status Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        public int OrderNumber { get; set; }

        private bool _isSelected = false;

        [Computed]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private int _completedSubTasksCount = 0;

        [Computed]
        public int CompletedSubTasksCount
        {
            get { return _completedSubTasksCount; }
            set { SetProperty(ref _completedSubTasksCount, value); }
        }

        [Computed]
        public ObservableCollection<SubTask> SubTasks { get; set; } = new ObservableCollection<SubTask>();

        [Computed]
        public ObservableCollection<Contributor> Contributors { get; set; } = new ObservableCollection<Contributor>();

        private double _progress;

        [Exportable]
        [Computed]
        public double Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public double GetProgress()
        {
            Progress = 0D;
            if (SubTasks.Count == 0 && Status != Status.Completed) return Progress;

            if (Status == Status.Completed)
            {
                Progress = 100D;
                return Progress;
            }

            if (SubTasks.Count == 0) return Progress;

            double itemWeight = 100 / SubTasks.Count;
            CompletedSubTasksCount = 0;

            foreach (var item in SubTasks)
            {
                if (item.Status == Status.Completed)
                {
                    Progress += itemWeight;
                    CompletedSubTasksCount++;
                }
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
