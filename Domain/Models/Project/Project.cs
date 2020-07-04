using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using Domain.Models.Comments;
using Domain.Models.Tasks;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Domain.Models.Project
{
    [Table("projects")]
    [Serializable]
    public class Project : BindableBase, IEntity, IUpdatableProgress, IStatus
    {

        public Project()
        {

        }

        public Project(Project project)
        {
            ID = project.ID;
            Header = project.Header;
            Content = project.Content;
            SubmitionDate = project.SubmitionDate;
            DueDate = project.DueDate;
            Priority = project.Priority;
            Status = project.Status;
            OrderNumber = project.OrderNumber;
            Progress = project.Progress;
            Tasks = project.Tasks;
            Contributors = project.Contributors;
            Comments = project.Comments;
            CompletedTasksCount = project.CompletedTasksCount;
        }

        [Key]
        public string ID { get; set; }

        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Default;
        public Status Status { get; set; } = Status.InProgress;
        public int OrderNumber { get; set; }

        private double _progress;

        [Computed]
        public double Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        private ObservableCollection<ProjectTask> _tasks = new ObservableCollection<ProjectTask>();

        [Computed]
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        [Computed]
        public ObservableCollection<Contributor> Contributors { get; set; } = new ObservableCollection<Contributor>();

        [Computed]
        public ObservableCollection<IComment> Comments { get; set; } = new ObservableCollection<IComment>();

        private int _completedTasksCount = 0;

        [Computed]
        public int CompletedTasksCount
        {
            get { return _completedTasksCount; }
            set { SetProperty(ref _completedTasksCount, value); }
        }

        public double GetProgress()
        {
            Progress = 0D;
            if (Status == Status.Completed)
            {
                Progress = 100D;
               
            }

            if (Tasks.Count == 0) return Progress;

            double itemWeight = 100 / Tasks.Count;
            CompletedTasksCount = 0;

            foreach (var item in Tasks)
            {

                if (item.Status == Status.Completed)
                {
                    if (Status != Status.Completed) Progress += itemWeight;
                    CompletedTasksCount++;
                    continue;
                }

                if (item.SubTasks.Count == 0) continue;

                double itemSubWeight = itemWeight / item.SubTasks.Count;

                if (Status != Status.Completed) Progress += itemWeight / 100 * item.GetProgress(); 
            }

            if (CompletedTasksCount == Tasks.Count) Progress = 100D;
           
            return Math.Round(Progress, 0);
        }

        public bool HasString(string param, ViewType viewType)
        {
            var type = viewType switch
            {
                ViewType.All => (int)Status,
                ViewType.Archived => (int)Status.Archived,
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
