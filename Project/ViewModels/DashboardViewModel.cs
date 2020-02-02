using Core.DataBase;
using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Idoneus.ViewModels.Base;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Idoneus.ViewModels
{
    public class DashboardViewModel : BaseViewModel
   
    {

        #region Observable Items

        public ObservableCollection<IProject> Projects { get; set; }
        public ObservableCollection<TodaysTask> Tasks { get; set; }
        public ObservableCollection<SubTask> UpcomingTasks { get; set; } = new ObservableCollection<SubTask>();
     

        #endregion // Observable Items

        #region ICommand Properties

        public ICommand ShowAddTaskPopupCommand { get; set; }
        public ICommand CleanCompletedTasksCommand { get; set; }
        public ICommand SelectTaskCommand { get; set; }
        public ICommand AddNewDailyTaskCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }

        #endregion // ICommand Properties

        #region Public Properties

        #region Visibility

        public bool AddTaskPopupVisible { get; set; } = false;

        #endregion // Visibility

        #region Info Box

        public decimal TotalTasksProgress { get; set; }

        public int TotalProjectCount { get; set; }
        public int ActiveTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }

        #endregion // Info Box

        public DateTime CurrentDate { get; } = DateTime.Now;
        public DateTime TargetDate
        {
            get => _targetDate;
            set
            {
                if (_targetDate != value)
                {
                    _targetDate = value;
                    SetUpcomingTasks();
                }
            }
        }

        #endregion // Public Properties

        #region Private properties

        private DateTime _targetDate;

        #endregion // Private properties

        #region Constructor

        public DashboardViewModel()
        {
            _targetDate = DateTime.Now;
            _targetDate = _targetDate.AddDays(7);

            GetData();
            SetUpCommands();
            InitTest();


            CheckForUpdates();
        }

        #endregion // Constructor

        #region ICommand Methods

        private void SetUpCommands ()
        {
            SelectTaskCommand = new ParameterizedRelayCommand<TodaysTask>(SelectTask);
            CleanCompletedTasksCommand = new RelayCommand(CleanCompletedTasks);
            AddNewDailyTaskCommand = new ParameterizedRelayCommand<string>(AddNewDailyTask);
            OpenProjectCommand = new ParameterizedRelayCommand<IProject>(OpenProject);
        }

        private void OpenProject(IProject project)
        {
            var index = Projects.IndexOf(project);
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Projects, index);
        }


        private void CleanCompletedTasks ()
        {
            var list = new List<TodaysTask>();
            for (int i = Tasks.Count-1; i >= 0; i--)
            {
                if (Tasks[i].IsCompleted)
                {
                    list.Add(Tasks[i]);
                    Tasks.Remove(Tasks[i]);
                    
                }
            }

            DBHelper.DeleteAllTodaysTask(list);
        }

        private void AddNewDailyTask (string taskContent)
        {
            if (!StringHelper.CanUse(taskContent)) return;

            var task = new TodaysTask
            {
                Content = taskContent.Trim(),
                SubmitionDate = DateTime.Now,
                DueDate = DateTime.Now
            };

            DBHelper.InsertTodaysTask(task);

            Tasks.Insert(0, task);

            //AddTaskPopupVisible ^= true;


        }

        private void SelectTask(TodaysTask task)
        {
            var index = Tasks.IndexOf(task);

            // If task is completed - move it to the end. Otherwise, move to front
            var newIndex = task.IsCompleted ? Tasks.Count - 1 : 0;

            DBHelper.UpdateTodaysTask(task);
            Tasks.Move(index, newIndex);
        }

        #endregion  ICommand Methods

        #region Private Methods

        private void GetData ()
        {
            FileHelper.CreateFolderIfNotExist("./Database");

            if (!FileHelper.FileExists("./Database/db.db")) NotifyDBMissing();

            FileHelper.CreateFolderIfNotExist("./Projects");
                

            Projects = new ObservableCollection<IProject>(DBHelper.GetProjects(ViewType.All));

            foreach (var project in Projects)
            {
                FileHelper.CreateFolderIfNotExist(project.Path);
                FileHelper.CreateFolderIfNotExist($"{project.Path}{Path.DirectorySeparatorChar}_Comments");
            }

            // Counters
            UpdateCounters();

            Tasks = DBHelper.GetTodaysTasks (DateTime.Now);
            SetUpcomingTasks();
        }

        private void SetUpcomingTasks ()
        {
            UpcomingTasks.Clear();
            UpcomingTasks = DBHelper.GetUpcomingTasks(TargetDate);
        }

        private void UpdateCounters ()
        {
            ActiveTasksCount = DBHelper.GetAllTasks(false);
            OverdueTasksCount = DBHelper.GetOverdueTasks();
            CompletedTasksCount = DBHelper.GetAllTasks(true);
            TotalTasksProgress = IntHelper.GetRoundedPercentage( (ActiveTasksCount + CompletedTasksCount), CompletedTasksCount);
            //TotalTasksProgress = GetTotalTasksProgress();
            TotalProjectCount = DBHelper.GetPublishedProjectsCount();
        }

        private void NotifyDBMissing ()
        {
            // Give message
            try
            {
                DBHelper.CreateTablesIfNotExist();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion // Private Methods

        #region Test Methods

        public ICommand TestCommand { get; set; }

        private void InitTest ()
        {
            TestCommand = new ParameterizedRelayCommand<string>(TestMethod);
        }

        private void TestMethod(string test)
        {
            Debug.WriteLine("test hit");
            Debug.WriteLine($"param: {test}");
            
           
        }


        #endregion

        #region Updater

        private async System.Threading.Tasks.Task CheckForUpdates ()
        {
            using var mgr = UpdateManager.GitHubUpdateManager("https://github.com/kvarcas91/Idoneus");
            var currentVersion = $"Status: {mgr.Status}";
            Console.WriteLine(currentVersion);
            await mgr.Result.UpdateApp();
            


        }

        #endregion // updater

    }
}
