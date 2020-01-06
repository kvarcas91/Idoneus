using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public static class FakeData
    {

        public static ObservableCollection<IProject> GetProjects ()
        {

            var projects = new ObservableCollection<IProject>
            {
                new Project
                {
                    Header = "Do something",
                    Progress = 20,
                    Priority = Utils.Priority.High,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 100,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text"
                 },

                  new Project
                {
                    Header = "Do something",
                    Progress = 20,
                    Priority = Utils.Priority.Low,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 100,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text"
                 },

                  new Project
                {
                    Header = "Do something",
                    Progress = 20,
                    Priority = Utils.Priority.High,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 100,
                    Priority = Utils.Priority.Default,
                    Content = "Second content from Eddie designed to be a long text"
                 },

                  new Project
                {
                    Header = "Do something",
                    Progress = 20,
                    Priority = Utils.Priority.High,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 100,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text"
                 },

                  new Project
                {
                    Header = "Do something",
                    Progress = 20,
                    Priority = Utils.Priority.High,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 99,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text"
                 },

                  new Project
                {
                    Header = "Do something",
                    Progress = 0,
                    Priority = Utils.Priority.High,
                    Content = "First content from Eddie"
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 49,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text"
                 }
            };

            return projects;
        }

        public static ObservableCollection<ITask> GetTasks()
        {
            var tasks = new ObservableCollection<ITask>
            {
               new Task
               {
                   Content = "First task",
                   IsCompleted = false
               },

                new Task
               {
                   Content = "Second task",
                   IsCompleted = false
               },

                 new Task
               {
                   Content = "third task",
                   IsCompleted = true
               }
            };


            return tasks;
        }

        public static ObservableCollection<Note> GetNotes()
        {
            var notes = new ObservableCollection<Note>
            {
                new Note
                {
                    Content = "my very first note",
                    SubmitionDate = DateTime.UtcNow
                },

                new Note
                {
                    Content = "my very first note",
                    SubmitionDate = DateTime.MinValue
                }
            };

            return notes;
        }
    }
}
