using Core.DataBase;
using Core.DataModels;
using Core.Utils;
using Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public ICommand AddNewNoteCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }

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

            GetData();
            SetUpCommands();
            InitTest();
        }

        #endregion // Constructor

        #region ICommand Methods

        private void SetUpCommands ()
        {
            SelectTaskCommand = new ParameterizedRelayCommand<ITask>(SelectTask);
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

        private void AddNewNote(string noteContent)
        {
            if (string.IsNullOrWhiteSpace(noteContent)) return;

            Notes.Insert(0, new Note
            {
                Content = noteContent.Trim(),
                SubmitionDate = DateTime.Now
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

        private void GetData ()
        {
            FileHelper.CreateFolderIfNotExist("./Database");

            if (!FileHelper.FileExists("./Database/db.db"))
                DBHelper.CreateTablesIfNotExist();

            //Projects = FakeData.GetProjects();
            Projects = new ObservableCollection<IProject>(DBHelper.GetProjects(ViewType.All));
            Tasks = FakeData.GetTasks();
            Notes = FakeData.GetNotes();



            //Projects = new ObservableCollection<IProject>();
            //Tasks = new ObservableCollection<ITask>();
            //Notes = new ObservableCollection<Note>();
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
