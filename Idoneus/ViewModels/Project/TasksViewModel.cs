using Common;
using Common.EventAggregators;
using Domain.Extentions;
using Domain.Models;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
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
            set { SetProperty(ref _selectedTask, value); }
        }

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

        private ObservableCollection<ProjectTask> _tasks;
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
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

        #endregion // Delegates


        public TasksViewModel(IEventAggregator eventAggregator, ProjectRepository repository)
        {
            _eventAggregator = eventAggregator;
            _repository = repository;

            SelectedContributors = new ObservableCollection<Contributor>();
            SelectedTasks = new ObservableCollection<ProjectTask>();
            AllContributors = new ObservableCollection<Contributor>(_repository.GetAllContributors());

            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);

            TaskViewType = new List<string>() { "All", "In Progress", "Completed", "Archived", "Delayed" };
        }

        #region Methods

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
