using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class ProjectsViewModel : BindableBase, INavigationAware
    {
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
            Debug.WriteLine("OnVaigatedTo", nameof(ProjectsViewModel));
        }
    }
}
