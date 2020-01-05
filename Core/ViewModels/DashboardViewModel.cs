using Core.DataModels;
using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {

        public ObservableCollection<Project> Projects { get; set; }

        public ObservableCollection<Task> Tasks { get; set; }

        public DashboardViewModel()
        {
            Projects = new ObservableCollection<Project> 
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
        }

    }
}
