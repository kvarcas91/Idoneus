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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Idoneus.ViewModels
{
    public class ProjectsViewModel : BindableBase, INavigationAware
    {

        private Project _project;

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

        private readonly ProjectRepository _projectRepository;
        private readonly IEventAggregator _eventAggregator;

        public ProjectsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _projectRepository = new ProjectRepository();
            Tabs = new ObservableCollection<UserControl>
            {
                new Overview(),
                new Details(),
                new Tasks()
            };

            InitData();
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
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _allProjects = new ObservableCollection<Project>(_projectRepository.GetProjects());
                        Projects = _allProjects.Clone();

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

        private void OnItemClicked(Project project)
        {
            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);

            ProjectTitle = project.Header;
            _eventAggregator.GetEvent<SendCurrentProject<Project>>().Publish(project);
        }

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("OnVaigatedFrom", nameof(ProjectsViewModel));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
          
            _project = navigationContext.Parameters["project"] as Project;
            if(_project != null)
            {
                ProjectTitle = _project.Header;
                _eventAggregator.GetEvent<SendCurrentProject<Project>>().Publish(_project);
            }
            InitData();
        }

        #endregion // Navigation
    }
}
