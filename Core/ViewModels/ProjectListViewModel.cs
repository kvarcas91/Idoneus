using Core.DataBase;
using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Windows.Input;

namespace Core.ViewModels
{
    public class ProjectListViewModel : BaseViewModel
    {
        
        private static ProjectListViewModel _instance;
        public static ProjectListViewModel Instance
        {
            get
            {
                if (_instance == null) _instance = new ProjectListViewModel();
                return _instance;
            }
            set => _instance = value;
        }

        #region Observable Collections

        public ObservableCollection<IProject> Projects { get; set; }
        //public IList<IElement> Tasks { get; set; }
        public ObservableCollection<IComment> Comments { get; set; }

        #endregion // Observable Collections

        #region Icommand Properties

        public ICommand TestCommand { get; set; }
        public ICommand SelectProjectCommand { get; set; }
        public ICommand CollapseProjectListCommand { get; set; }
        public ICommand ExpandTaskPanelCommand { get; set; }
        public ICommand AddNewTaskCommand { get; set; }
        public ICommand SelectTaskCommand { get; set; }
        public ICommand ExpandTaskCommand { get; set; }
        public ICommand ExpandSubTaskPanelCommand { get; set; }
        public ICommand AddSubTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }

        #endregion // Icommand Properties

        #region Icommand Methods

        private void AddNewTask ()
        {
            if (!StringHelper.CanUse(NewTaskContent)) return;

            var task = new Task
            {
                Content = NewTaskContent,
                Priority = NewTaskPriority,
                DueDate = NewTaskSelectedDate
            };

            DBHelper.InsertTask(task, CurrentProject.ID);
            CurrentProject.Tasks.Add(task);
            NewTaskContent = string.Empty;
            UpdateCounters();
        }

        private void SelectTask (IElement param)
        {
            //task.IsCompleted ^= true;
            if (param is Task task)
            {
                task.UpdateProgress();
                DBHelper.UpdateTask(task);
            }
            if (param is SubTask subTask)
            {
                //subTask.IsCompleted ^= true;
                var parent = CurrentProject.Tasks[subTask.ParentIndex];
                ((Task)parent).UpdateProgress();
                DBHelper.UpdateTask(subTask);

            }
            UpdateCounters();
        }

        private void ExpandTask (IElement param)
        {
            if (param is Task task)
            {
                task.IsExpanded ^= true;
                if (!task.IsExpanded) task.IsAddSubTaskPanelVisible = false;
            }
        }

        private void ExpandSubTaskPanel(IElement param)
        {
            if (param is Task task)
            {
                task.IsAddSubTaskPanelVisible ^= true;
            }
        }

        private void AddSubTask(IElement param)
        {

            if (!StringHelper.CanUse(SubTaskContent)) return;
            var subTask = new SubTask
            {
                Content = SubTaskContent,
                DueDate = SubTaskDueTime,
                Priority = SubTaskPriority
            };

            if (param is Task task)
            {
                task.IsAddSubTaskPanelVisible ^= true;
                task.IsExpanded ^= true;
                subTask.ParentIndex = CurrentProject.Tasks.IndexOf(task);
                task.AddElement(subTask);
                task.UpdateProgress();
                UpdateCounters();
                DBHelper.InsertSubTask(subTask, task.ID);
            }
        }

        private void DeleteTask (IElement param)
        {
            if (param is ITask)
            {
                DBHelper.DeleteTask(param);
                CurrentProject.Tasks.Remove(param);
                
            }
            if (param is SubTask subtask)
            {
                DBHelper.DeleteSubTask(subtask);
                var task = (Task)CurrentProject.Tasks[subtask.ParentIndex];
                task.SubTasks.Remove(subtask);
            }
            ((Task)param).UpdateProgress();
            UpdateCounters();
        }

        #endregion // Icommand Methods

        #region Public Properties

        public IProject CurrentProject { get; set; }

        #region New Task Properties

        public Priority NewTaskPriority { get; set; } = Priority.Low;
        public string NewTaskContent { get; set; } = string.Empty;
        public DateTime NewTaskSelectedDate { get; set; } = DateTime.UtcNow;

        #endregion // New Task Properties

        #region New SubTask Command

        public string SubTaskContent { get; set; } = string.Empty;
        public Priority SubTaskPriority { get; set; } = Priority.Low;
        public DateTime SubTaskDueTime { get; set; } = DateTime.UtcNow;

        #endregion

        #region Project Properties

      

        #endregion // Project Properties

        #region Counters



        #endregion // Counters

        #region Visilibity

        public bool IsProjectListSideBarExpanded { get; set; } = true;
        public bool IsControlExpanded { get; set; } = false;

        #endregion // Visibility

        #region Info Box

        public double? TotalSubTasksProgress
        {
            get => (double)IntHelper.GetRoundedPercentage((double)(CompletedSubTasksCount + OverdueSubTasksCount), (double)CompletedSubTasksCount);
            set => TotalSubTasksProgress = value;
        }
        /// <summary>
        /// gets total 
        /// </summary>
        public int? CompletedSubTasksCount { get; set; } = 0;
        public int? OverdueSubTasksCount { get; set; } = 0;

        #endregion // Info Box

        #endregion // Public Properties

        #region Private Properties

       

        #endregion // Private Properties

        #region Private Methods

        private void SetUpCommands ()
        {
            CollapseProjectListCommand = new RelayCommand(CollapseProjectList);
            ExpandTaskPanelCommand = new RelayCommand(ExpandTaskPanel);
            AddNewTaskCommand = new RelayCommand(AddNewTask);
            SelectProjectCommand = new ParameterizedRelayCommand<IProject>(SelectProject);
            SelectTaskCommand = new ParameterizedRelayCommand<IElement>(SelectTask);
            ExpandTaskCommand = new ParameterizedRelayCommand<IElement>(ExpandTask);
            ExpandSubTaskPanelCommand = new ParameterizedRelayCommand<IElement>(ExpandSubTaskPanel);
            AddSubTaskCommand = new ParameterizedRelayCommand<IElement>(AddSubTask);
            DeleteTaskCommand = new ParameterizedRelayCommand<IElement>(DeleteTask);
        }

        private void SelectProject (IProject project)
        {
            if (CurrentProject != project)
            {
                CurrentProject = project;
                UpdateCounters();
            }
        }

        private void UpdateCounters ()
        {
            ((Project)CurrentProject)?.UpdateProgress();
            CompletedSubTasksCount = ((Project)CurrentProject)?.GetCompletedSubTaskCount();
            OverdueSubTasksCount = ((Project)CurrentProject)?.GetOverdueSubTaskCount();
        }

        private void CollapseProjectList ()
        {
            IsProjectListSideBarExpanded ^= true;
        }

        private void ExpandTaskPanel()
        {
            IsControlExpanded ^= true;
        }

        #endregion // Private Methods

        #region Constructor

        public ProjectListViewModel()
        {

            Instance = this;

            Projects = new ObservableCollection<IProject>(DBHelper.GetProjects(ViewType.All));

            var index = IoC.Get<ApplicationViewModel>().Parameters;
            if (index != null)
            {
                
                CurrentProject = Projects[(int)index];
            }

            IsProjectListSideBarExpanded = (CurrentProject == null);

            SetUpCommands();

            UpdateCounters();

            InitTest();
        }

        

        #endregion // Constructor


        #region Test

        private void InitTest ()
        {
            TestCommand = new ParameterizedRelayCommand<IProject>(Test);
        }

        private void Test (IProject project)
        {
            CurrentProject = project;
        }

        #endregion // Test
    }
}
