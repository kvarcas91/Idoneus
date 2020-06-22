using Domain.Models.Project;
using Prism.Mvvm;
using Prism.Regions;
using System.Diagnostics;

namespace Idoneus.ViewModels
{
    public class ProjectsViewModel : BindableBase, INavigationAware
    {

        private Project _project;



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
        }

        #endregion // Navigation
    }
}
