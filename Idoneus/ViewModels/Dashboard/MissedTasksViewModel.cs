using Common.EventAggregators;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class MissedTasksViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly DailyTasksRepository _repository;
        private ObservableCollection<TodaysTask> _tasks;
        public ObservableCollection<TodaysTask> Tasks
        {
            get { return _tasks; }
            private set { SetProperty(ref _tasks, value); }
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(GetData));

        public MissedTasksViewModel(IEventAggregator eventAggregator)
        {
            Tasks = new ObservableCollection<TodaysTask>();
            _eventAggregator = eventAggregator;
            _repository = new DailyTasksRepository();
            GetData();
        }



        private void GetData()
        {
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => Tasks.Clear());
                var tasks = _repository.GetMissedTasks();
                App.Current.Dispatcher.Invoke(() => Tasks.AddRange(tasks));
                _eventAggregator.GetEvent<SendMessageToDailyTasks>().Publish((-1, Tasks.Count));
            });
        }
    }
}
