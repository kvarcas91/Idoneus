using Domain.Models.Project;
using Domain.Repository;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Idoneus.ViewModels
{
    public class ProjectsViewModel : BindableBase, INavigationAware
    {

        private Project _project;

        private ObservableCollection<Project> _projects;
        private readonly ProjectRepository _projectRepository;

        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        public ProjectsViewModel()
        {
            _projectRepository = new ProjectRepository();
            Projects = new ObservableCollection<Project>(_projectRepository.GetProjects());
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
            if (Projects == null) Projects = new ObservableCollection<Project>(_projectRepository.GetProjects());
        }

        #endregion // Navigation
    }
}
