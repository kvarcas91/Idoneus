using Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.DataModels
{
    public class Project : IProject
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
        public double Progress { get; set; }
        public string Path { get; set; }
        public int CompletedTasksCount { get; set; }

        public void SetCompletedTasksCount ()
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
    }
}
