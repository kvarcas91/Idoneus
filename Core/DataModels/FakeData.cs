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
                    Header = "First serious project. What Do I want",
                    Progress = 89,
                    Priority = Utils.Priority.Default,
                    Content = "First content from Eddie. This is very high priority and we will see how long it can go. Apparently not long enough",
                    Tasks = GetRandomTasks()
                },

                 new Project
                 {
                    Header = "Do More stuff",
                    Progress = 100,
                    Priority = Utils.Priority.Medium,
                    Content = "Second content from Eddie designed to be a long text",
                    Tasks = GetRandomTasks()
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
                    Progress = 47,
                    Priority = Utils.Priority.Medium,
                    Content = "Some random text here",
                    SubmitionDate = DateTime.Now,
                    Tasks = GetRandomTasks()
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
                    Content = "First content from Eddie",
                    Tasks = GetRandomTasks()
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
                   IsCompleted = false,
                   Priority = Utils.Priority.Default,
                   Progress = 38
               },

                new Task
               {
                   Content = "Second task",
                   IsCompleted = false,
                   Priority = Utils.Priority.Medium,
                   Progress = 98
               },

                 new Task
               {
                   Content = "third task",
                   IsCompleted = true,
                   Priority = Utils.Priority.High,
                   Progress = 2
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

        public static Project GetProject ()
        {
            return new Project
            {
                Header = "My First Project And I do what I want to do",
                Content = "lovely content about something biiiig and nice",
                Progress = 46,
                Priority = Utils.Priority.Medium,
                SubmitionDate = DateTime.MinValue,
                DueDate = DateTime.Now,
                Tasks = GetRandomTasks()
            };
        }

        public static ObservableCollection<IElement> GetRandomTasks ()
        {
            Random random = new Random();
            var tasks = GetTasks();
            var count = random.Next(tasks.Count);

            var output = new ObservableCollection<IElement>();
            for (int i = 0; i < count; i++)
            {
                output.Add(tasks[random.Next(tasks.Count)]);
            }

            return output;
        }
    }
}
