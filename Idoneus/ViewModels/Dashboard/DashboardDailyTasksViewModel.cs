using Common.EventAggregators;
using Domain.Extentions;
using Domain.Models.Tasks;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Idoneus.ViewModels
{
    public class DashboardDailyTasksViewModel : BindableBase
    {
        private ObservableCollection<TodaysTask> _allTasks;
        private ObservableCollection<TodaysTask> _tasks;
        public ObservableCollection<TodaysTask> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }


        public DashboardDailyTasksViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SendMessageToDailyTasks<ObservableCollection<TodaysTask>>>().Subscribe(MessageReceived);
        }

        private void MessageReceived(ObservableCollection<TodaysTask> tasks)
        {

            _allTasks = tasks.Clone();
            Tasks = tasks;
        }

    }
}
