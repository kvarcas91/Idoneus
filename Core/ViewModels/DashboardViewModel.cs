using Core.DataModels;
using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }

        public ICommand ShowAddTaskPopupCommand { get; set; }
        public ICommand CleanCompletedTasksCommand { get; set; }
        public ICommand TestCommand { get; set; }

        public bool AddTaskPopupVisible { get; set; } = false;

        public double TotalProgress { get; set; } = 49;

        public DashboardViewModel()
        {


            TestCommand = new ParameterizedRelayCommand<Task>(Test);



            Projects = new ObservableCollection<Project>();

            //Projects = new ObservableCollection<Project> 
            //{ 
            //    new Project
            //    {
            //        Header = "Do something",
            //        Progress = 20,
            //        Priority = Utils.Priority.High,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 100,
            //        Priority = Utils.Priority.Medium,
            //        Content = "Second content from Eddie designed to be a long text"
            //     },

            //      new Project
            //    {
            //        Header = "Do something",
            //        Progress = 20,
            //        Priority = Utils.Priority.Low,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 100,
            //        Priority = Utils.Priority.Medium,
            //        Content = "Second content from Eddie designed to be a long text"
            //     },

            //      new Project
            //    {
            //        Header = "Do something",
            //        Progress = 20,
            //        Priority = Utils.Priority.High,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 100,
            //        Priority = Utils.Priority.Default,
            //        Content = "Second content from Eddie designed to be a long text"
            //     },

            //      new Project
            //    {
            //        Header = "Do something",
            //        Progress = 20,
            //        Priority = Utils.Priority.High,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 100,
            //        Priority = Utils.Priority.Medium,
            //        Content = "Second content from Eddie designed to be a long text"
            //     },

            //      new Project
            //    {
            //        Header = "Do something",
            //        Progress = 20,
            //        Priority = Utils.Priority.High,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 99,
            //        Priority = Utils.Priority.Medium,
            //        Content = "Second content from Eddie designed to be a long text"
            //     },

            //      new Project
            //    {
            //        Header = "Do something",
            //        Progress = 0,
            //        Priority = Utils.Priority.High,
            //        Content = "First content from Eddie"
            //    },

            //     new Project
            //     {
            //        Header = "Do More stuff",
            //        Progress = 49,
            //        Priority = Utils.Priority.Medium,
            //        Content = "Second content from Eddie designed to be a long text"
            //     }
            //};


            Tasks = new ObservableCollection<Task>();

            Tasks = new ObservableCollection<Task>
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
            Tasks.Add(new Task
            {
                Content = "test from UI"
            }
           );

            ShowAddTaskPopupCommand = new RelayCommand(ShowAddTaskPopup);
            CleanCompletedTasksCommand = new RelayCommand(CleanCompletedTasks);
        }


        private void ShowAddTaskPopup ()
        {
            AddTaskPopupVisible ^= true;
            Debug.WriteLine("pop up");
            //Tasks.Add(new Task
            //{
            //    Content = "test from UI"
            //}
            //);
        }

        private void CleanCompletedTasks ()
        {
            for (int i = Tasks.Count-1; i >= 0; i--)
            {
                if (Tasks[i].IsCompleted) Tasks.Remove(Tasks[i]);
            }
        }


        #region Test Methods

        private void Test (Task task)
        {
            Debug.WriteLine("Test hit");
           
           
            Tasks.OrderBy(i => i.IsCompleted);
            
        }


        #endregion


    }
}
