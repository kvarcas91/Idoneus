using Common;
using Common.EventAggregators;
using Domain.Extentions;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Base;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class TasksViewModel : BindableBase
    {

        #region Local members

        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectRepository _repository;
        private Action _deselectContributors;
        private bool _isTaskEditable = false;

        #endregion // Local members

        #region Properties

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { SetProperty(ref _currentProject, value); }
        }

        private ProjectTask _selectedTask;
        public ProjectTask SelectedTask
        {
            get { return _selectedTask; }
            set { SetProperty(ref _selectedTask, value); SetSubTasks(); }
        }

        private IEntity _newTask;

        #region New Task

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _priority;
        public string Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        private DateTime _dueDate = DateTime.Now.AddDays(7);
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        #endregion // New Task

        #region Tasks

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private bool _isAllTasksSelected;
        public bool IsAllTasksSelected
        {
            get { return _isAllTasksSelected; }
            set { SetProperty(ref _isAllTasksSelected, value); SelectAllTasks(); }
        }

        private List<string> _taskViewType;
        public List<string> TaskViewType
        {
            get => _taskViewType;
            set { SetProperty(ref _taskViewType, value); }
        }

        private string _selectedTaskViewType = "All";
        public string SelectedTaskViewType
        {
            get => _selectedTaskViewType;
            set { SetProperty(ref _selectedTaskViewType, value); HandleViewTypeSelect(); }
        }

        #endregion // Tasks

        #region SubTasks

        private string _searchSubTaskText = string.Empty;
        public string SearchSubTaskText
        {
            get => _searchSubTaskText;
            set { SetProperty(ref _searchSubTaskText, value); HandleSubTaskSearch(); }
        }

        private bool _isAllSubTasksSelected;
        public bool IsAllSubTasksSelected
        {
            get { return _isAllSubTasksSelected; }
            set { SetProperty(ref _isAllSubTasksSelected, value); SelectAllSubTasks(); }
        }

        private List<string> _subTaskViewType;
        public List<string> SubTaskViewType
        {
            get => _subTaskViewType;
            set { SetProperty(ref _subTaskViewType, value); }
        }

        private string _selectedSubTaskViewType = "All";
        public string SelectedSubTaskViewType
        {
            get => _selectedSubTaskViewType;
            set { SetProperty(ref _selectedSubTaskViewType, value); HandleSubTaskViewTypeSelect(); }
        }

        #endregion // SubTasks

        #region Contributors

        private int _selectedContributorCount = 0;
        public int SelectedContributorCount
        {
            get { return _selectedContributorCount; }
            set { SetProperty(ref _selectedContributorCount, value); }
        }

        private string _selectedContributor;
        public string SelectedContributor
        {
            get { return _selectedContributor; }
            set { SetProperty(ref _selectedContributor, value); }
        }

        #endregion // Description / Contributors

        #endregion

        public ObservableCollection<Contributor> AllContributors { get; set; }

        private ObservableCollection<Contributor> _selectedContributors;
        public ObservableCollection<Contributor> SelectedContributors
        {
            get { return _selectedContributors; }
            set { SetProperty(ref _selectedContributors, value); }
        }
        public ObservableCollection<ProjectTask> SelectedTasks { get; set; }
        public ObservableCollection<SubTask> SelectedSubTasks { get; set; }

        private ObservableCollection<ProjectTask> _tasks;
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        private ObservableCollection<SubTask> _subTasks;
        public ObservableCollection<SubTask> SubTasks
        {
            get { return _subTasks; }
            set { SetProperty(ref _subTasks, value); }
        }

        #region Delegates

        #region Contributors / Details

        private DelegateCommand _deleteSelectedContributorsCommand;
        public DelegateCommand DeleteSelectedContributorsCommand => _deleteSelectedContributorsCommand ?? (_deleteSelectedContributorsCommand = new DelegateCommand(DeleteSelectedContributors));

        private DelegateCommand _unselectContributorsCommand;
        public DelegateCommand UnselectContributorsCommand => _unselectContributorsCommand ?? (_unselectContributorsCommand = new DelegateCommand(UnselectContributors));

        private DelegateCommand _addContributorCommand;
        public DelegateCommand AddContributorCommand => _addContributorCommand ?? (_addContributorCommand = new DelegateCommand(AddContributor));

        #endregion // Contributors / Details

        #region Tasks

        private DelegateCommand<ProjectTask> _checkTaskCommand;
        public DelegateCommand<ProjectTask> CheckTaskCommand => _checkTaskCommand ?? (_checkTaskCommand = new DelegateCommand<ProjectTask>(CheckTask));

        private DelegateCommand _deleteSelectedTasksCommand;
        public DelegateCommand DeleteSelectedTasksCommand => _deleteSelectedTasksCommand ?? (_deleteSelectedTasksCommand = new DelegateCommand(DeleteSelectedTasks));

        private DelegateCommand _completeSelectedTasksCommand;
        public DelegateCommand CompleteSelectedTasksCommand => _completeSelectedTasksCommand ?? (_completeSelectedTasksCommand = new DelegateCommand(CompleteSelectedTasks));

        private DelegateCommand _reopenSelectedTasksCommand;
        public DelegateCommand ReopenSelectedTasksCommand => _reopenSelectedTasksCommand ?? (_reopenSelectedTasksCommand = new DelegateCommand(ReopenSelectedTasks));

        private DelegateCommand _unselectTasksCommand;
        public DelegateCommand UnselectTasksCommand => _unselectTasksCommand ?? (_unselectTasksCommand = new DelegateCommand(UnselectTasks));

        #endregion // Tasks

        #region SubTasks

        private DelegateCommand<SubTask> _checkSubTaskCommand;
        public DelegateCommand<SubTask> CheckSubTaskCommand => _checkSubTaskCommand ?? (_checkSubTaskCommand = new DelegateCommand<SubTask>(CheckSubTask));

        private DelegateCommand _deleteSelectedSubTasksCommand;
        public DelegateCommand DeleteSelectedSubTasksCommand => _deleteSelectedSubTasksCommand ?? (_deleteSelectedSubTasksCommand = new DelegateCommand(DeleteSelectedSubTasks));

        private DelegateCommand _completeSelectedSubTasksCommand;
        public DelegateCommand CompleteSelectedSubTasksCommand => _completeSelectedSubTasksCommand ?? (_completeSelectedSubTasksCommand = new DelegateCommand(CompleteSelectedSubTasks));

        private DelegateCommand _reopenSelectedSubTasksCommand;
        public DelegateCommand ReopenSelectedSubTasksCommand => _reopenSelectedSubTasksCommand ?? (_reopenSelectedSubTasksCommand = new DelegateCommand(ReopenSelectedSubTasks));

        private DelegateCommand _unselectSubTasksCommand;
        public DelegateCommand UnselectSubTasksCommand => _unselectSubTasksCommand ?? (_unselectSubTasksCommand = new DelegateCommand(UnselectSubTasks));

        #endregion // SubTasks

        private DelegateCommand<string> _addNewTaskCommand;
        public DelegateCommand<string> AddNewTaskCommand => _addNewTaskCommand ?? (_addNewTaskCommand = new DelegateCommand<string>(AddNewTask));

        private DelegateCommand _closeDialogCommand;
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

        private DelegateCommand _confirmNewTaskCommand;
        public DelegateCommand ConfirmNewTaskCommand => _confirmNewTaskCommand ?? (_confirmNewTaskCommand = new DelegateCommand(ConfirmNewTask));

        private DelegateCommand<ITask> _editTaskCommand;
        public DelegateCommand<ITask> EditTaskCommand => _editTaskCommand ?? (_editTaskCommand = new DelegateCommand<ITask>(EditTask));

        #endregion // Delegates


        public TasksViewModel(IEventAggregator eventAggregator, ProjectRepository repository)
        {
            _eventAggregator = eventAggregator;
            _repository = repository;

            SelectedContributors = new ObservableCollection<Contributor>();

            SelectedTasks = new ObservableCollection<ProjectTask>();
            SelectedSubTasks = new ObservableCollection<SubTask>();

            AllContributors = new ObservableCollection<Contributor>(_repository.GetAllContributors());

            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);

            TaskViewType = new List<string>() { "All", "In Progress", "Completed", "Archived", "Missed" };
            SubTaskViewType = new List<string>() { "All", "In Progress", "Completed", "Archived", "Missed" };
        }

        #region Methods

        private void AddNewTask(string type)
        {
            if (type.Equals("TASK")) _newTask ??= new ProjectTask();
            else
            {
                if (SelectedTask == null)
                {
                    PublishSnackBar("Select Task first!");
                    return;
                }
                _newTask ??= new SubTask();
            }
            var dialog = DialogHost.OpenDialogCommand;
            dialog.Execute(null, null);

        }

        private void ConfirmNewTask()
        {
            if (!IsNewProjectValid() || CurrentProject == null || _newTask == null) return;

            Priority priority = (Priority)Enum.Parse(typeof(Priority), Priority);

            if (_newTask is ProjectTask task)
            {
                if (!_isTaskEditable) task.ID = Guid.NewGuid().ToString();
                task.DueDate = DueDate;
                task.Content = Content;
                task.Priority = priority;
                if (!_isTaskEditable) task.ParentID = CurrentProject.ID;

                if (_isTaskEditable)
                {
                    var currentProjectTaskIndex = CurrentProject.Tasks.IndexOf(CurrentProject.Tasks.Where(t => t.ID.Equals(task.ID)).FirstOrDefault());
                    var taskIndex = Tasks.IndexOf(CurrentProject.Tasks.Where(t => t.ID.Equals(task.ID)).FirstOrDefault());
                    if (currentProjectTaskIndex >= 0 && taskIndex >= 0)
                    {
                        CurrentProject.Tasks.RemoveAt(currentProjectTaskIndex);
                        Tasks.RemoveAt(taskIndex);
                    }
                }
                
                CurrentProject.Tasks.Add(task);
                Tasks.Add(task);

                Task.Run(() => {
                    if (!_isTaskEditable) _repository.Insert(task, "tasks");
                    else _repository.Update(task);
                    
                    App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                    PublishSnackBar($"Tasks have been {(_isTaskEditable ? "updated" : "inserted")}!");
                    _newTask = null;
                    _isTaskEditable = false;
                });

            }
            if (_newTask is SubTask subTask)
            {
                if (!_isTaskEditable) subTask.ID = Guid.NewGuid().ToString();
                subTask.DueDate = DueDate;
                subTask.Content = Content;
                subTask.Priority = priority;
                if (!_isTaskEditable) subTask.ParentID = SelectedTask.ID;

                if (_isTaskEditable)
                {
                    var SelectedTaskIndex = SelectedTask.SubTasks.IndexOf(SelectedTask.SubTasks.Where(t => t.ID.Equals(subTask.ID)).FirstOrDefault());
                    var taskIndex = SubTasks.IndexOf(SelectedTask.SubTasks.Where(t => t.ID.Equals(subTask.ID)).FirstOrDefault());
                    if (taskIndex >= 0 && SelectedTaskIndex >= 0)
                    {
                        SelectedTask.SubTasks.RemoveAt(SelectedTaskIndex);
                        SubTasks.RemoveAt(taskIndex);
                    }
                }

                SubTasks.Add(subTask);
                SelectedTask.SubTasks.Add(subTask);

                SelectedTask.GetProgress();

                Task.Run(() => {
                    if (!_isTaskEditable) _repository.Insert(subTask, "subtasks");
                    else _repository.Update(subTask);
                    App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                    PublishSnackBar($"Tasks have been {(_isTaskEditable ? "updated" : "inserted")}!");
                    _newTask = null;
                    _isTaskEditable = false;
                });
            }

            CloseDialog();
        }

        private bool IsNewProjectValid()
        {
            if (!ValidationHelper.Validate(Priority, Content) || DueDate < DateTime.Now)
            {
                return false;
            }

            return true;
        }

        private void CloseDialog()
        {
            var dialog = DialogHost.CloseDialogCommand;
            dialog.Execute(null, null);

            ClearTaskData();
        }

        private void ClearTaskData()
        {
            Content = string.Empty;
            DueDate = DateTime.Now.AddDays(7);
            Priority = string.Empty;
        }

        private void EditTask(ITask paramTask)
        {
            _isTaskEditable = true;
            _newTask = paramTask;
            Content = paramTask.Content;
            DueDate = paramTask.DueDate;
            Priority = paramTask.Priority.ToString();
            var type = (paramTask is ProjectTask) ? "TASK" : "SUBTASK";
            AddNewTask(type);
        }

        #region Tasks

        private void HandleViewTypeSelect()
        {
           
            Task.Run(() =>
            {
                if (SelectedTaskViewType.Equals("All"))
                {
                    App.Current.Dispatcher.Invoke(() => Tasks = CurrentProject.Tasks.Clone());
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        HandleSearch();
                        return;
                    }

                    return;
                }
                Tasks = new ObservableCollection<ProjectTask>(_repository.SortByViewType(CurrentProject.Tasks, SelectedTaskViewType));
                if (!string.IsNullOrEmpty(SearchText))
                {
                    HandleSearch();
                    return;
                }

            });

        }

        private void HandleSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                HandleViewTypeSelect();
                return;
            }

            Task.Run(() =>
            {
                Enum.TryParse(SelectedTaskViewType.Replace(" ", string.Empty), out ViewType type);

                App.Current.Dispatcher.Invoke(() => Tasks.Clear());

                foreach (var item in CurrentProject.Tasks)
                {
                    if (item.HasString(SearchText, type)) App.Current.Dispatcher.Invoke(() => Tasks.Add(item));
                }
            });

        }

        private void SelectAllTasks()
        {
            if (!_isAllTasksSelected)
            {
                UnselectTasks();
                return;
            }

            SelectedTasks.Clear();
            foreach (var task in Tasks)
            {
                SelectedTasks.Add(task);
                task.IsSelected = true;
            }
        }

        private void CheckTask(ProjectTask task)
        {
            if (task.IsSelected) SelectedTasks.Add(task);
            else SelectedTasks.Remove(task);
        }

        private void DeleteSelectedTasks()
        {
            foreach (var task in SelectedTasks)
            {
                var mTask = CurrentProject.Tasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                if (mTask == null)
                {
                    PublishSnackBar("Something went wrong...");
                    return;
                }

                CurrentProject.Tasks.Remove(mTask);
                Tasks.Remove(task);

            }

            Task.Run(() => {
                _repository.DeleteTasks(SelectedTasks);
                IsAllTasksSelected = false;
                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                PublishSnackBar("Tasks have been deleted!");
            }); 
        }

        private void ReopenSelectedTasks()
        {
            Task.Run(() =>
            {
                foreach (var task in SelectedTasks)
                {

                    var mTask = CurrentProject.Tasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                    if (mTask == null)
                    {
                        PublishSnackBar("something went wrong..");
                        return;
                    }

                    if (mTask.Status != Status.Completed) continue;

                    task.Status = Status.InProgress;
                    task.GetProgress();

                    mTask.Status = Status.InProgress;
                    mTask.GetProgress();

                    _repository.Update(mTask);

                }

                IsAllTasksSelected = false;
                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
               
                PublishSnackBar("Tasks have been completed!");
            });
        }

        private void CompleteSelectedTasks()
        {
            Task.Run(() =>
            {
                foreach (var task in SelectedTasks)
                {
                    if (task.Status == Status.Completed) continue;
                    var mTask = CurrentProject.Tasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                    if (mTask == null)
                    {
                        PublishSnackBar("something went wrong..");
                        return;
                    }
                    task.Status = Status.Completed;
                    task.GetProgress();

                    mTask.Status = Status.Completed;
                    mTask.GetProgress();


                    _repository.Update(mTask);

                }
                IsAllTasksSelected = false;

                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                PublishSnackBar("Tasks have been completed!");
            });
        }

        private void UnselectTasks()
        {
            foreach (var task in Tasks)
            {
                task.IsSelected = false;
            }
            SelectedTasks.Clear();
        }

        #endregion // Tasks

        #region SubTasks

        private void HandleSubTaskViewTypeSelect()
        {

            Task.Run(() =>
            {
                if (SelectedSubTaskViewType.Equals("All"))
                {
                    App.Current.Dispatcher.Invoke(() => SubTasks = SelectedTask.SubTasks.Clone());
                    if (!string.IsNullOrEmpty(SearchSubTaskText))
                    {
                        HandleSubTaskSearch();
                        return;
                    }

                    return;
                }

                SubTasks = new ObservableCollection<SubTask>(_repository.SortByViewType(SelectedTask.SubTasks, SelectedSubTaskViewType));
                if (!string.IsNullOrEmpty(SearchSubTaskText))
                {
                    HandleSubTaskSearch();
                    return;
                }

            });

        }

        private void HandleSubTaskSearch()
        {
            if (string.IsNullOrEmpty(SearchSubTaskText))
            {
                HandleSubTaskViewTypeSelect();
                return;
            }

            Task.Run(() =>
            {
                Enum.TryParse(SelectedSubTaskViewType.Replace(" ", string.Empty), out ViewType viewType);

                App.Current.Dispatcher.Invoke(() => SubTasks.Clear());

                foreach (var item in SelectedTask.SubTasks)
                {
                    var type = viewType switch
                    {
                        ViewType.All => (int)item.Status,
                        ViewType.Archived => (int)Status.Archived,
                        _ => (int)viewType,
                    };
                    if (item.HasString(SearchSubTaskText, type)) App.Current.Dispatcher.Invoke(() => SubTasks.Add(item));
                }
            });

        }

        private void SelectAllSubTasks()
        {
            if (!_isAllSubTasksSelected)
            {
                UnselectSubTasks();
                return;
            }

            SelectedSubTasks.Clear();
            foreach (var subTask in SubTasks)
            {
                SelectedSubTasks.Add(subTask);
                subTask.IsSelected = true;
            }
        }

        private void CheckSubTask(SubTask task)
        {
            if (task.IsSelected) SelectedSubTasks.Add(task);
            else SelectedSubTasks.Remove(task);
        }

        private void DeleteSelectedSubTasks()
        {
            foreach (var task in SelectedSubTasks)
            {
                var mTask = SelectedTask.SubTasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                if (mTask == null)
                {
                    PublishSnackBar("Something went wrong...");
                    return;
                }

                SelectedTask.SubTasks.Remove(mTask);
                SubTasks.Remove(task);

            }

            Task.Run(() => {
                _repository.DeleteTasks(SelectedTasks);
                IsAllTasksSelected = false;
                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                PublishSnackBar("Tasks have been deleted!");
            });
        }

        private void ReopenSelectedSubTasks()
        {
            Task.Run(() =>
            {
                foreach (var task in SelectedSubTasks)
                {

                    var mTask = SelectedTask.SubTasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                    if (mTask == null)
                    {
                        PublishSnackBar("something went wrong..");
                        return;
                    }

                    if (mTask.Status != Status.Completed) continue;

                    task.Status = Status.InProgress;
                    // task.GetProgress();

                    mTask.Status = Status.InProgress;
                    //mTask.GetProgress();

                    _repository.Update(mTask);

                }
                SelectedTask.GetProgress();

                IsAllSubTasksSelected = false;
                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));

                PublishSnackBar("Tasks have been completed!");
            });
        }

        private void CompleteSelectedSubTasks()
        {
            Task.Run(() =>
            {
                foreach (var task in SelectedSubTasks)
                {
                    if (task.Status == Status.Completed) continue;
                    var mTask = SelectedTask.SubTasks.Where(x => x.ID.Equals(task.ID)).FirstOrDefault();
                    if (mTask == null)
                    {
                        PublishSnackBar("something went wrong..");
                        return;
                    }
                    task.Status = Status.Completed;

                    mTask.Status = Status.Completed;

                    _repository.Update(mTask);

                }
                SelectedTask.GetProgress();
                IsAllSubTasksSelected = false;

                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(CurrentProject));
                PublishSnackBar("Tasks have been completed!");
            });
        }

        private void UnselectSubTasks()
        {
            foreach (var task in SubTasks)
            {
                task.IsSelected = false;
            }
            SelectedSubTasks.Clear();
        }

        private void SetSubTasks()
        {
            if (SelectedTask != null)
            {
                SubTasks.Clear();
                SubTasks.AddRange(SelectedTask.SubTasks);
            }
        }

        #endregion // SubTasks

        #region Contributors / Details

        private void DeleteSelectedContributors()
        {
            
            Task.Run(() =>
            {
                _repository.UnassignTaskContributors(SelectedContributors, SelectedTask.ID);

                for (int i = SelectedContributors.Count - 1; i >= 0; i--)
                {
                    App.Current.Dispatcher.Invoke(() => SelectedTask.Contributors.Remove(SelectedContributors[i]));
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    SelectedContributors.Clear();
                    SelectedContributorCount = SelectedContributors.Count;
                    PublishSnackBar("Contributors were removed!");
                });
            });
        }

        private void UnselectContributors()
        {
            _deselectContributors();
            SelectedContributors.Clear();
            SelectedContributorCount = SelectedContributors.Count;
        }

        private void AddContributor()
        {
            if (SelectedTask == null)
            {
                PublishSnackBar("Select task first!");
                return;
            }

            if (string.IsNullOrEmpty(SelectedContributor))
            {
                PublishSnackBar("Cannot add this person...");
                return;
            }
            var names = SelectedContributor.Trim().Split(' ');
            if (names.Length < 2)
            {
                PublishSnackBar("Please enter full name");
                return;
            }
            var contributor = _repository.GetContributor(names);

            if (contributor == null)
            {
                contributor = new Contributor
                {
                    FirstName = names[0],
                    LastName = names[1],
                    ID = Guid.NewGuid().ToString()
                };

                _repository.Insert(contributor, "contributors");
                AllContributors.Add(contributor);
            }

            if (SelectedTask.Contributors.Where(c => c.ID.Equals(contributor.ID)).FirstOrDefault() != null)
            {
                PublishSnackBar("Contributor already added!");
                return;
            }

            if (_currentProject != null && !_repository.AssignTaskContributor(SelectedTask.ID, contributor.ID))
            {
                PublishSnackBar("Failed to assign contributor...");
                return;
            }

            PublishSnackBar("Contributor has been added!");
            SelectedTask.Contributors.Add(contributor);
            SelectedContributor = string.Empty;
        }

        public void SetSelectedContributors(IEnumerable<Contributor> list)
        {
            SelectedContributors.Clear();
            SelectedContributors.AddRange(list);
            SelectedContributorCount = SelectedContributors.Count;
        }

        public void SetDeselectAction(Action action)
        {
            _deselectContributors = action;
        }

        #endregion // Contributors / Details

        #region Common

     

        private void ProjectReceived(Project project)
        {
            if (project == null) return;
            CurrentProject = project;
            Tasks = new ObservableCollection<ProjectTask>(CurrentProject.Tasks.Clone());
            SubTasks = new ObservableCollection<SubTask>();
            if (project == null) return;

            //Task.Run(() =>
            //{
            //    CurrentProject = _repository.GetProject(project.ID);
            //});
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        #endregion Common

        #endregion // Methods

    }
}
