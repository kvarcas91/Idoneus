using Core.DataBase;
using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using Idoneus.ViewModels.Base;
using Idoneus.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Windows;

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

        public ObservableCollection<IData> RelatedFiles { get; set; } = new ObservableCollection<IData>();

        #endregion // Observable Collections

        #region Icommand Properties

        public ICommand TestCommand { get; set; }
        // Project Commands
        public ICommand SelectProjectCommand { get; set; }
        public ICommand CollapseProjectListCommand { get; set; }
        public ICommand EditProjectCommand { get; set; }
        public ICommand DeleteProjectCommand { get; set; }
        public ICommand ExpandProjectControlCommand { get; set; }

        // Task Commands
        public ICommand ExpandTaskPanelCommand { get; set; }
        public ICommand AddNewTaskCommand { get; set; }
        public ICommand SelectTaskCommand { get; set; }
        public ICommand ExpandTaskCommand { get; set; }
        public ICommand ExpandSubTaskPanelCommand { get; set; }
        public ICommand AddSubTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }

        // Contributors Commands
        public ICommand AddContributorsCommand { get; set; }

        // Comments Commands
        public ICommand AddCommentCommand { get; set; }
        public ICommand ExpandCommentCommand { get; set; }
        public ICommand UpdateCommentCommand { get; set; }
        public ICommand DeleteCommentCommand { get; set; }

        // File Commands
        public ICommand FileListItemClickCommand { get; set; }
        public ICommand ShowProjectDirectoryCommand { get; set; }
        public ICommand NavigateFolderBackCommand { get; set; }

        #endregion // Icommand Properties

        #region Icommand Methods

        #region Project

        private void EditProject(IProject project)
        {
            ExpandProjectControl();
            var updatedProject = AddProjectPrompt.Show((Core.DataModels.Project)project);
            if (updatedProject == null) return;
            CurrentProject = updatedProject;
            DBHelper.UpdateProject((Core.DataModels.Project)CurrentProject);
        }

        private void ExpandProjectControl()
        {
            IsExpanded ^= true;
        }

        private void DeleteProject()
        {
            DBHelper.DeleteProject((Core.DataModels.Project)CurrentProject);
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Dashboard);
        }

        #endregion // Project

        #region Task

        private void AddTask()
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

        private void SelectTask(IElement param)
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

        private void ExpandTask(IElement param)
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

        private void EditTask(IElement param)
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

        private void DeleteTask(IElement param)
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

        #endregion // Task

        #region Contributors

        private void AddContributors()
        {
            CurrentProject.Contributors = AddContributorPrompt.ShowDialog(CurrentProject.ID, CurrentProject.Contributors);
        }

        #endregion // Contributors

        #region Comment

        private void AddComment()
        {
            if (CommentContent.Trim(' ').Length == 0) return;

            Comment comment = new Comment
            {
                Content = CommentContent
            };
            DBHelper.InsertComment(comment, CurrentProject.ID);
            CurrentProject.Comments.Insert(0, comment);

            CommentContent = string.Empty;


        }

        private void ExpandComment(IComment comment)
        {
            if (comment != null && PreviewComment != comment)
            {
                IsExpandedCommentViewVisible = true;
                PreviewComment = comment;
            }
            else
            {
                IsExpandedCommentViewVisible = false;
                PreviewComment = null;
            }

        }

        private void UpdateComment()
        {
            DBHelper.UpdateComment(PreviewComment);
            ExpandComment(null);
        }

        private void DeleteComment(IComment comment)
        {
            CurrentProject.Comments.Remove(comment);
            DBHelper.DeleteComment(comment);
        }

        #endregion // Comment

        #region Files

        private void FileListItemClick (IData component)
        {
            if (component is IFolder folder)
            {
                CanNavigateBack = true;
                currentPath = Path.Combine(currentPath, folder.Name);
                SetUpRelatedFiles();
                return;
            }
            Process.Start($"{Directory.GetParent(Assembly.GetExecutingAssembly().Location)}{$"{Path.DirectorySeparatorChar}{component.Path}"}");
        }

        private void ShowProjectDirectory ()
        {
            CanNavigateBack = false;
            currentPath = CurrentProject.Path;
            SetUpRelatedFiles();
        }

        private void NavigateFolderBack ()
        {
            if (!FileHelper.CanNavigateBack(currentPath, CurrentProject.Path)) return;

            string[] directories = currentPath.Split(Path.DirectorySeparatorChar);
            if (directories.Length > 1) directories.SetValue(string.Empty, directories.Length - 1);

            currentPath = Path.Combine(directories);
            SetUpRelatedFiles();
        }

        #endregion // Files

        #endregion // Icommand Methods

        #region Public Properties

        public IProject CurrentProject { get; set; }
        public bool IsExpanded { get; set; } = false;

        // Files
        public bool CanNavigateBack { get; set; } = false;

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

        #region Comment Properties

        public string CommentContent { get; set; }
        public bool IsExpandedCommentViewVisible { get; set; } = false;
        public IComment PreviewComment { get; set; }

        #endregion // Comment Properties

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
        private string currentPath = string.Empty;
        

    #endregion // Private Properties

        #region Private Methods

        private void SetUpCommands ()
        {

            // Project
            CollapseProjectListCommand = new RelayCommand(CollapseProjectList);
            SelectProjectCommand = new ParameterizedRelayCommand<IProject>(SelectProject);
            EditProjectCommand = new ParameterizedRelayCommand<IProject>(EditProject);
            ExpandProjectControlCommand = new RelayCommand(ExpandProjectControl);
            DeleteProjectCommand = new RelayCommand(DeleteProject);

            // Task
            ExpandTaskPanelCommand = new RelayCommand(ExpandTaskPanel);
            AddNewTaskCommand = new RelayCommand(AddTask);
            SelectTaskCommand = new ParameterizedRelayCommand<IElement>(SelectTask);
            ExpandTaskCommand = new ParameterizedRelayCommand<IElement>(ExpandTask);
            ExpandSubTaskPanelCommand = new ParameterizedRelayCommand<IElement>(ExpandSubTaskPanel);
            AddSubTaskCommand = new ParameterizedRelayCommand<IElement>(AddSubTask);
            DeleteTaskCommand = new ParameterizedRelayCommand<IElement>(DeleteTask);
            EditTaskCommand = new ParameterizedRelayCommand<IElement>(EditTask);

            // Contributors
            AddContributorsCommand = new RelayCommand(AddContributors);

            // Comments
            AddCommentCommand = new RelayCommand(AddComment);
            ExpandCommentCommand = new ParameterizedRelayCommand<IComment>(ExpandComment);
            UpdateCommentCommand = new RelayCommand(UpdateComment);
            DeleteCommentCommand = new ParameterizedRelayCommand<IComment>(DeleteComment);

            // Files
            FileListItemClickCommand = new ParameterizedRelayCommand<IData>(FileListItemClick);
            ShowProjectDirectoryCommand = new RelayCommand(ShowProjectDirectory);
            NavigateFolderBackCommand = new RelayCommand(NavigateFolderBack);
        }

        private void SelectProject (IProject project)
        {
            if (CurrentProject != project)
            {
                CurrentProject = project;
                UpdateCounters();
                InitializeDueDates();
                currentPath = CurrentProject.Path;
                SetUpRelatedFiles();
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

        private void InitializeDueDates()
        {
            if (CurrentProject == null) return;

            NewTaskSelectedDate = CurrentProject.DueDate;
            SubTaskDueTime = CurrentProject.DueDate;
        }

        private void SetUpRelatedFiles ()
        {
            RelatedFiles.Clear();

            foreach (var item in FileHelper.GetFolderContent(currentPath))
            {
                RelatedFiles.Add(item);
                Console.WriteLine(item.Path);
            }
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
                if ((int)index == -1)
                {
                    CurrentProject = Projects[Projects.Count - 1];
                }
                else CurrentProject = Projects[(int)index];

                currentPath = CurrentProject.Path;
            }

            IsProjectListSideBarExpanded = (CurrentProject == null);

            SetUpCommands();

            UpdateCounters();

            InitializeDueDates();

            SetUpRelatedFiles();

            InitTest();
        }



        #endregion // Constructor

        #region Drag/drop

        public void OnDropOuterFile(string[] files)
        {
            if (files == null) return;
            foreach (var file in files)
            {
                FileAttributes attr = 0;
                try
                {
                    attr = File.GetAttributes(file);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Sorry, coulnd't drop file. It might not exist");
                    return;
                }
                IData component;
                if (attr.HasFlag(FileAttributes.Directory)) component = new RelatedFolder(file);
                else component = new RelatedFile(file);
                if (!component.Move(currentPath))
                {
                    MessageBox.Show("Cannot move file..");
                    return;
                }
                //ShowFolderContent(CurrentPath);
                RelatedFiles.Add(component);
            }
        }

        public void OnDrop(IData sourceItem, IData destinationItem)
        {
            if (Equals(sourceItem, destinationItem)) return;
            if (destinationItem is IFile) return;

            sourceItem.Move(destinationItem.Path);
            RelatedFiles.Remove(sourceItem);
            //ShowFolderContent(CurrentPath);
        }

        #endregion // Drag/drop

        #region Test

        private void InitTest ()
        {
            TestCommand = new ParameterizedRelayCommand<IProject>(Test);
        }

        private void Test (IProject project)
        {
            //CurrentProject = project;
            //var key = Registry.ClassesRoot.GetValue("Directory\\shellex\\ContextMenuHandlers\\");
            

        }

        #endregion // Test
    }
}
