using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardDailyTasksViewModel : BindableBase
    {
        private string _header = "Dashboard";
        public string Header
        {
            get => _header;
            set { SetProperty(ref _header, value); }
        }
    }
}
