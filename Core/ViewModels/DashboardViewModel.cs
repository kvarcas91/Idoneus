using Core.DataBase;
using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
   
    {

        #region Observable Items

        public ObservableCollection<IProject> Projects { get; set; }
        public ObservableCollection<TodaysTask> Tasks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }

        #endregion // Observable Items

        #region ICommand Properties

        public ICommand ShowAddTaskPopupCommand { get; set; }
        public ICommand CleanCompletedTasksCommand { get; set; }
        public ICommand SelectTaskCommand { get; set; }
        public ICommand AddNewDailyTaskCommand { get; set; }
        public ICommand AddNewNoteCommand { get; set; }
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

        #endregion // Public Properties

        #region Constructor

        public DashboardViewModel()
        {

            GetData();
            SetUpCommands();
            InitTest();
        }

        #endregion // Constructor

        #region ICommand Methods

        private void SetUpCommands ()
        {
            SelectTaskCommand = new ParameterizedRelayCommand<TodaysTask>(SelectTask);
            ShowAddTaskPopupCommand = new RelayCommand(ShowAddTaskPopup);
            CleanCompletedTasksCommand = new RelayCommand(CleanCompletedTasks);
            AddNewDailyTaskCommand = new ParameterizedRelayCommand<string>(AddNewDailyTask);
            AddNewNoteCommand = new ParameterizedRelayCommand<string>(AddNewNote);
            OpenProjectCommand = new ParameterizedRelayCommand<IProject>(OpenProject);
        }

        private void OpenProject(IProject project)
        {
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Projects, project);
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

        private void AddNewNote(string noteContent)
        {
            if (!StringHelper.CanUse(noteContent)) return;

            Notes.Insert(0, new Note
            {
                Content = noteContent.Trim(),
                SubmitionDate = DateTime.Now
            });

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
                

            Projects = new ObservableCollection<IProject>(DBHelper.GetProjects(ViewType.All));

            // Counters
            UpdateCounters();

            Tasks = DBHelper.GetTodaysTasks (DateTime.Now);
            Notes = FakeData.GetNotes();


        }

        private void UpdateCounters ()
        {
            ActiveTasksCount = DBHelper.GetAllTasks(false);
            OverdueTasksCount = DBHelper.GetOverdueTasks();
            CompletedTasksCount = DBHelper.GetAllTasks(true);
            TotalTasksProgress = IntHelper.GetPercentage( (ActiveTasksCount + CompletedTasksCount), CompletedTasksCount);
            //TotalTasksProgress = GetTotalTasksProgress();
            TotalProjectCount = DBHelper.GetPublishedProjectsCount();
        }

        private void NotifyDBMissing ()
        {
            // Give message
            DBHelper.CreateTablesIfNotExist();
        }

        private int GetTotalTasksProgress ()
        {
            if (CompletedTasksCount == 0) return 0;

            return (CompletedTasksCount * 100) / (ActiveTasksCount + CompletedTasksCount);
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

    }
}
