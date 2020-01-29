using Core.DataBase;
using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Idoneus.ViewModels.Base;
using Project.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Idoneus.ViewModels
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
        public ICommand EditTaskCommand { get; set; }
        public ICommand AddContributorsCommand { get; set; }

        #endregion // Icommand Properties

        #region Icommand Methods

        private void AddTask ()
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
            NewTaskPriority = Priority.Low;
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
            editableTask = null;

            SubTaskContent = string.Empty;
            SubTaskPriority = Priority.Low;

            if (param is Task task)
            {
                task.IsExpanded ^= true;
                if (!task.IsExpanded) task.IsAddSubTaskPanelVisible = false;
            }
        }

        private void ExpandSubTaskPanel(IElement param)
        {
            editableTask = null;

            SubTaskContent = string.Empty;
            SubTaskPriority = Priority.Low;

            if (param is Task task)
            {
                task.IsAddSubTaskPanelVisible ^= true;
            }
        }

        private void AddSubTask(IElement param)
        {

            if (!StringHelper.CanUse(SubTaskContent)) return;
            if (editableTask == null)
            {
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

                    DBHelper.InsertSubTask(subTask, task.ID);
                }
            }
            else
            {
                if (editableTask is Task task)
                { 
                    ((Task)editableTask).IsAddSubTaskPanelVisible = false;
                   
                    ((Task)editableTask).IsExpanded = false;

                    task.Content = SubTaskContent;
                    task.Priority = SubTaskPriority;
                    task.DueDate = SubTaskDueTime;
                }
                if (editableTask is SubTask subtask)
                {
                    var parentTask = (Task)CurrentProject.Tasks[((SubTask)editableTask).ParentIndex];
                    parentTask.IsAddSubTaskPanelVisible = false;
                    parentTask.IsExpanded = false;

                    subtask.Content = SubTaskContent;
                    subtask.Priority = SubTaskPriority;
                    subtask.DueDate = SubTaskDueTime;
                }

               
                DBHelper.UpdateTask(editableTask);
                editableTask = null;
            }

            UpdateCounters();
        }

        private void EditTask (IElement param)
        {
            editableTask = param;
            if (param is ITask task)
            {
               
                ((Task)task).IsAddSubTaskPanelVisible = true;

                SubTaskContent = task.Content;
                SubTaskPriority = task.Priority;
                SubTaskDueTime = task.DueDate;
                
            }
            if (param is SubTask subTask)
            {
                var parentTask = (Task)CurrentProject.Tasks[((SubTask)editableTask).ParentIndex];
                parentTask.IsAddSubTaskPanelVisible = true;

                SubTaskContent = subTask.Content;
                SubTaskPriority = subTask.Priority;
                SubTaskDueTime = subTask.DueDate;
            }
        }

        private void DeleteTask (IElement param)
        {
            if (param is ITask)
            {
                DBHelper.DeleteTask(param);
                CurrentProject.Tasks.Remove(param);
                ((Task)param).UpdateProgress();

            }
            if (param is SubTask subtask)
            {
                DBHelper.DeleteSubTask(subtask);
                var task = (Task)CurrentProject.Tasks[subtask.ParentIndex];
                task.SubTasks.Remove(subtask);
            }
           
            UpdateCounters();
        }

        private void AddContributors()
        {
            //int test = Prompt.ShowDialog("this is text", "this is caption");
            int test = PromptTest.ShowDialog(CurrentProject.Contributors);
            Console.WriteLine(test);
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

       
        /// <summary>
        /// gets total 
        /// </summary>
        public int? CompletedSubTasksCount { get; set; } = 0;
        public int? OverdueSubTasksCount { get; set; } = 0;

        public double? TotalSubTasksProgress
        {
            get
            {
                if ((CompletedSubTasksCount == null) || (OverdueSubTasksCount == null)) return 0;
                return (double)IntHelper.GetRoundedPercentage(((Core.DataModels.Project)CurrentProject).TotalSubTaskCount, (double)(CompletedSubTasksCount - OverdueSubTasksCount));
            }
            set => TotalSubTasksProgress = value;
        }

        #endregion // Info Box

        #endregion // Public Properties

        #region Private Properties

        private IElement editableTask = null;
        

    #endregion // Private Properties

        #region Private Methods

    private void SetUpCommands ()
        {
            CollapseProjectListCommand = new RelayCommand(CollapseProjectList);
            ExpandTaskPanelCommand = new RelayCommand(ExpandTaskPanel);
            AddNewTaskCommand = new RelayCommand(AddTask);
            SelectProjectCommand = new ParameterizedRelayCommand<IProject>(SelectProject);
            SelectTaskCommand = new ParameterizedRelayCommand<IElement>(SelectTask);
            ExpandTaskCommand = new ParameterizedRelayCommand<IElement>(ExpandTask);
            ExpandSubTaskPanelCommand = new ParameterizedRelayCommand<IElement>(ExpandSubTaskPanel);
            AddSubTaskCommand = new ParameterizedRelayCommand<IElement>(AddSubTask);
            DeleteTaskCommand = new ParameterizedRelayCommand<IElement>(DeleteTask);
            EditTaskCommand = new ParameterizedRelayCommand<IElement>(EditTask);
            AddContributorsCommand = new RelayCommand(AddContributors);
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
            ((Core.DataModels.Project)CurrentProject)?.UpdateProgress();
            CompletedSubTasksCount = ((Core.DataModels.Project)CurrentProject)?.GetCompletedSubTaskCount();
            OverdueSubTasksCount = ((Core.DataModels.Project)CurrentProject)?.GetOverdueSubTaskCount();
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
