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
        public IList<IElement> Comments { get; set; }
        public IList<IContributor> Contributors { get; set; }
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
        public int CompletedTasksCount { get; set; }

        private decimal _progress = 0;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void UpdateProgress ()
        {
            var count = 0;
           
            foreach (var item in Tasks)
            {
                if (item is ITask task)
                {
                    if (task.IsCompleted) count++;
                }
            }

            CompletedTasksCount = count;

            Progress = IntHelper.GetPercentage(Tasks.Count, CompletedTasksCount);

            //Progress = Tasks.Count == 0 ? 0 : (CompletedTasksCount * 100) / Tasks.Count;
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
            throw new NotImplementedException();
        }

        public bool AddPersons(IList<IPerson> person)
        {
            throw new NotImplementedException();
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

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
