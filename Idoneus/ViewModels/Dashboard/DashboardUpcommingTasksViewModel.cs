using Common.EventAggregators;
using Domain.Models.Tasks;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardUpcommingTasksViewModel : BindableBase
    {

        public ObservableCollection<ITask> _upcomingTasks;
        public ObservableCollection<ITask> UpcomingTasks
        {
            get { return _upcomingTasks; }
            set { SetProperty(ref _upcomingTasks, value); }
        }

        public DashboardUpcommingTasksViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SendMessageToUpcommingTasks<ObservableCollection<ITask>>>().Subscribe(MessageReceived);
        }

        private void MessageReceived(ObservableCollection<ITask> projects)
        {
            UpcomingTasks = projects;
            foreach (var item in projects)
            {
                Debug.WriteLine(item);
            }

        }

    }
}
