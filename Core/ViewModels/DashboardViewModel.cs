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

        #region Observable Items

        public ObservableCollection<IProject> Projects { get; set; }
        public ObservableCollection<ITask> Tasks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }

        #endregion // Observable Items

        #region ICommand Properties

        public ICommand ShowAddTaskPopupCommand { get; set; }
        public ICommand CleanCompletedTasksCommand { get; set; }
        public ICommand SelectTaskCommand { get; set; }
        public ICommand AddNewDailyTaskCommand { get; set; }

        #endregion // ICommand Properties

        #region Public Properties

        #region Visibility

        public bool AddTaskPopupVisible { get; set; } = false;

        #endregion // Visibility

        #region Info Box

        public double TotalTasksProgress
        {
            get => CompletedTasksCount * 100 / (CompletedTasksCount + OverdueTasksCount);
            set => TotalTasksProgress = value;
        }
        public int TotalProjectCount { get; set; } = 495;
        public int ActiveTasksCount { get; set; } = 2469;
        public int CompletedTasksCount { get; set; } = 120;
        public int OverdueTasksCount { get; set; } = 100;

        #endregion // Info Box

        public DateTime CurrentDate { get; } = DateTime.Now;

        #endregion // Public Properties

        #region Constructor

        public DashboardViewModel()
        {


            Projects = FakeData.GetProjects();
            Tasks = FakeData.GetTasks();
            //Notes = FakeData.GetNotes();

            //Projects = new ObservableCollection<IProject>();
            //Tasks = new ObservableCollection<ITask>();
            Notes = new ObservableCollection<Note>();

            SetUpCommands();
            initTest();
        }

        #endregion // Constructor

        #region ICommand Methods

        private void SetUpCommands ()
        {
            SelectTaskCommand = new ParameterizedRelayCommand<ITask>(SelectTask);
            ShowAddTaskPopupCommand = new RelayCommand(ShowAddTaskPopup);
            CleanCompletedTasksCommand = new RelayCommand(CleanCompletedTasks);
            AddNewDailyTaskCommand = new ParameterizedRelayCommand<string>(AddNewDailyTask);
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

        private void AddNewDailyTask (string taskContent)
        {
            if (string.IsNullOrWhiteSpace(taskContent)) return;
            
            Tasks.Insert(0, new Task
            {
                Content = taskContent.Trim()
            });

            AddTaskPopupVisible ^= true;


        }

        private void SelectTask(ITask task)
        {
            var index = Tasks.IndexOf(task);

            // If task is completed - move it to the end. Otherwise, move to front
            var newIndex = task.IsCompleted ? Tasks.Count - 1 : 0;

            Tasks.Move(index, newIndex);
        }

        #endregion  ICommand Methods

        #region Private Methods



        #endregion // Private Methods

        #region Test Methods

        public ICommand TestCommand { get; set; }

        private void initTest ()
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
