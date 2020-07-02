using Common.EventAggregators;
using Domain.Extentions;
using Domain.Models.Project;
using Domain.Repository;
using Idoneus.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Idoneus.ViewModels
{
    public class ProjectsViewModel : BindableBase, INavigationAware
    {

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { SetProperty(ref _currentProject, value); }
        }

        private ObservableCollection<Project> _allProjects;
        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        public ObservableCollection<UserControl> Tabs { get; set; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private string _projectTitle = string.Empty;
        public string ProjectTitle
        {
            get => _projectTitle;
            set { SetProperty(ref _projectTitle, value); HandleSearch(); }
        }

        private bool _isDataLoaded = false;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set { SetProperty(ref _isDataLoaded, value); }
        }

        private int _tasksCount = 0;
        public int TasksCount
        {
            get { return _tasksCount; }
            set { SetProperty(ref _tasksCount, value); }
        }

        private int _viewType = 0;
        public int ViewType
        {
            get { return _viewType; }
            set
            {
                SetProperty(ref _viewType, value);
            }
        }

        private DelegateCommand<Project> _selectProjectCommand;
        public DelegateCommand<Project> SelectProjectCommand => _selectProjectCommand ?? (_selectProjectCommand = new DelegateCommand<Project>(OnItemClicked));

        private DelegateCommand _editProjectCommand;
        public DelegateCommand EditProjectCommand => _editProjectCommand ?? (_editProjectCommand = new DelegateCommand(EditProject));

        private readonly ProjectRepository _projectRepository;
        private readonly IEventAggregator _eventAggregator;

        public ProjectsViewModel(IEventAggregator eventAggregator, ProjectRepository repository)
        {
            _eventAggregator = eventAggregator;
            _projectRepository = repository;
            Tabs = new ObservableCollection<UserControl>
            {
                new Overview(),
                new Details(),
                new Tasks()
            };

            //eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);
            eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Subscribe(NotifyChanged);

            InitData();
        }

        private void NotifyChanged(Project project)
        {
            if (CurrentProject == null || project == null) return;

            if (!CurrentProject.ID.Equals(project.ID)) return;

            CurrentProject = project;
            CurrentProject.GetProgress();
            ProjectReceived(project);
        }

        private void ProjectReceived(Project project)
        {
            if (project == null) return;
            var p = _allProjects.Where(x => x.ID.Equals(project.ID)).FirstOrDefault();
            if (p == null) return;

            var index = _allProjects.IndexOf(p);
            if (index < 0) return;

            _allProjects.RemoveAt(index);
            _allProjects.Insert(index, project);
            HandleSearch();
            SetTaskCount();
          
        }

        private void HandleSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                Projects.Clear();
                Projects.AddRange(_allProjects);
                return;
            }

            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => Projects.Clear());

                foreach (var item in _allProjects)
                {
                    if (item.HasString(SearchText, Common.ViewType.All)) App.Current.Dispatcher.Invoke(() => Projects.Add(item));
                }
            });

        }

        private void InitData()
        {
            IsDataLoaded = false;
            Task.Run(() =>
            {
                if (Projects == null)
                {
                    var projects = _projectRepository.GetProjects();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _allProjects = new ObservableCollection<Project>(projects.Clone());
                        Projects = new ObservableCollection<Project>(_allProjects.Clone());
                    }
                    );

                    IsDataLoaded = true;
                    return;
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    Projects.Clear();
                    Projects.AddRange(_projectRepository.GetProjects());
                    IsDataLoaded = true;
                });

            });

        }

        private void EditProject()
        {
            _eventAggregator.GetEvent<EditProjectRequest<Project>>().Publish(CurrentProject);
        }

        private void SetTaskCount()
        {
            if (CurrentProject == null)
            {
                TasksCount = 0;
                return;
            }
            TasksCount = CurrentProject.Tasks.Count;
        }

        private void OnItemClicked(Project project)
        {
            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);

            ProjectTitle = project.Header;
            CurrentProject = project;
            SetTaskCount();
            _eventAggregator.GetEvent<SendCurrentProject<Project>>().Publish(project);
        }

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            CurrentProject = null;
            _eventAggregator.GetEvent<SendCurrentProject<Project>>().Publish(null);
            SetTaskCount();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

            CurrentProject = navigationContext.Parameters["project"] as Project;
            if(CurrentProject != null)
            {
                ProjectTitle = CurrentProject.Header;
                _eventAggregator.GetEvent<SendCurrentProject<Project>>().Publish(CurrentProject);
                SetTaskCount();
            }
            InitData();
        }

        #endregion // Navigation
    }
}
