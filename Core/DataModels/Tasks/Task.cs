﻿using Core.Helpers;
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
    [Table("tasks")]
    public class Task : ITask, INotifyPropertyChanged
    {
        [Key]
        public long ID { get; set; }

        [Computed]
        public IList<IPerson> Contributors { get; set; } = new ObservableCollection<IPerson>();

        [Computed]
        public ObservableCollection<ISubTask> SubTasks { get; } = new ObservableCollection<ISubTask>();

        [Computed]
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }

        [Computed]
        public decimal Progress { get; set; }
        public uint OrderNumber { get; set; }
        public bool IsCompleted { get; set; }
        public string Content { get; set; }

        [Computed]
        public bool IsExpanded { get; set; }

        [Computed]
        public bool IsAddSubTaskPanelVisible { get; set; }

        [Computed]
        public int CompletedSubTasksCount { get; set; }


        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void UpdateProgress()
        {
            var completedSubtaskCount = 0;

            foreach (var item in SubTasks)
            {
                if (item is ISubTask task)
                {
                    if (task.IsCompleted) completedSubtaskCount++;
                }
            }

            CompletedSubTasksCount = completedSubtaskCount;

            if (SubTasks.Count != 0 && Equals(completedSubtaskCount, SubTasks.Count)) IsCompleted = true;

            if (IsCompleted) Progress = 100;
            else
            {
                Progress = IntHelper.GetPercentage(SubTasks.Count, CompletedSubTasksCount);
            }
        }


        public bool AddElement(IElement element)
        {
            if (element is ISubTask task) SubTasks.Add(task);
            return true;
        }

        public bool AddElements(IList<IElement> elements)
        {
            foreach (var element in elements)
            {
                AddElement(element);
            }
            return true;
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

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
