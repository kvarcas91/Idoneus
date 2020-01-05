using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {

        public ObservableCollection<string> Projects { get; set; }

        public ObservableCollection<string> Tasks { get; set; }

        public DashboardViewModel()
        {
            Projects = new ObservableCollection<string> 
            { 
                "Do something about it",
                "Maybe later"
            };

            Tasks = new ObservableCollection<string>
            {
                "Finish this page",
                "Do some nice data bindings"
            };
        }

    }
}
