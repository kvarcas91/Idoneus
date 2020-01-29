using Core.Helpers;
using Core.Utils;
using Dapper.Contrib.Extensions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core.DataModels
{
    [ImplementPropertyChanged]
    public class Project : IProject, INotifyPropertyChanged
    {
        public string Header { get; set; }
        public bool IsArchived { get; set; }
        public ObservableCollection<IElement> Comments { get; set; } = new ObservableCollection<IElement>();
        public ObservableCollection<IContributor> Contributors { get; set; } = new ObservableCollection<IContributor>();
        public ObservableCollection<IElement> Tasks { get; set; }
        public long ID { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public decimal Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    //NotifyPropertyChanged();
                }
            }
        }
        public string Path { get; set; }

        [Computed]
        public int TotalSubTaskCount { get; set; } = 0;

        [Computed]
        public int CompletedTasksCount { get; set; }

        private decimal _progress = 0;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void UpdateProgress ()
        {

            CompletedTasksCount = 0;
            var taskWeight = Tasks.Count == 0 ? 100 : IntHelper.GetPercentage(Tasks.Count, 1);
            var progressCount = 0.0M;

            foreach (var item in Tasks)
            {
                var task = ((Task)item);

                if (task.IsCompleted) CompletedTasksCount++;

                var subtaskCount = task.SubTasks.Count;

                if (subtaskCount == 0)
                {
                    if (task.IsCompleted) progressCount += taskWeight;
                    continue;
                }

                var completedSubtaskCount = 0;
                foreach (var subtask in task.SubTasks)
                {
                    if (subtask.IsCompleted) completedSubtaskCount++;
                }

                var taskProgress = IntHelper.GetPercentage(subtaskCount, completedSubtaskCount, (double)taskWeight);
                progressCount += taskProgress;

            }
            Progress = IntHelper.Round(progressCount);


        }

        public bool AddElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool AddElements(IList<IElement> elements)
        {
            throw new NotImplementedException();
        }

        public bool AddPerson(IPerson person)
        {
            if (person is IContributor contributor) Contributors.Add(contributor);
            return true;
        }

        public bool AddPersons(IList<IPerson> persons)
        {
            foreach (var person in persons)
            {
                AddPerson(person);
            }
            return true;
        }

        public bool RemoveElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool RemovePerson(IPerson person)
        {
            throw new NotImplementedException();
        }

        public bool UpdateElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePerson(IPerson person)
        {
            throw new NotImplementedException();
        }

        public int GetCompletedSubTaskCount ()
        {
            var count = 0;
            TotalSubTaskCount = 0;
            foreach (var element in Tasks)
            {
                
                foreach (var subtask in ((Task)element).SubTasks)
                {
                    if (subtask.IsCompleted) count++;
                    TotalSubTaskCount++;
                }
            }
            return count;
        }

        

        public int GetOverdueSubTaskCount()
        {
            var count = 0;
            foreach (var element in Tasks)
            {

                foreach (var subtask in ((Task)element).SubTasks)
                {
                    if (DateTime.Compare(subtask.DueDate, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))) < 0) count++;
                }
            }
            return count;
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
