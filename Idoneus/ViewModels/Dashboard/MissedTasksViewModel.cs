using Common.EventAggregators;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private DelegateCommand<TodaysTask> _selectTaskCommand;
        public DelegateCommand<TodaysTask> SelectTaskCommand => _selectTaskCommand ?? (_selectTaskCommand = new DelegateCommand<TodaysTask>(SelectTask));

        private DelegateCommand _deleteCompletedTasksCommand;
        public DelegateCommand DeleteCompletedTasksCommand => _deleteCompletedTasksCommand ?? (_deleteCompletedTasksCommand = new DelegateCommand(DeleteCompletedTasks));

        private DelegateCommand _completeAllCommand;
        public DelegateCommand CompleteAllCommand => _completeAllCommand ?? (_completeAllCommand = new DelegateCommand(CompleteAll));

        public MissedTasksViewModel(IEventAggregator eventAggregator)
        {
            Tasks = new ObservableCollection<TodaysTask>();
            _eventAggregator = eventAggregator;
            _repository = new DailyTasksRepository();
            GetData();
        }

        private void SendSnackBarMessage(string message)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(message);
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

        private void SelectTask(TodaysTask task)
        {
            var index = Tasks.IndexOf(task);

            // If task is completed - move it to the end. Otherwise, move to front
            var newIndex = task.IsCompleted ? Tasks.Count - 1 : 0;

            //DBHelper.UpdateTodaysTask(task);
            var results = _repository.Update(task);
            if (results)
            {
                Tasks.Move(index, newIndex);
                CalculateProgress();
            }
        }

        private void CompleteAll()
        {
            Task.Run(() =>
            {
                var tempTasks = new List<TodaysTask>();
                foreach (var task in Tasks)
                {
                    task.IsCompleted = true;
                    tempTasks.Add(task);
                }
                var results = _repository.UpdateAll(Tasks);
                if (!results) SendSnackBarMessage("Something went wrong..");
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Tasks.Clear();
                        Tasks.AddRange(tempTasks);
                        CalculateProgress();
                    });
                }
            });

        }

        private void DeleteCompletedTasks()
        {
            var completedTasks = new List<TodaysTask>();
            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                if (Tasks[i].IsCompleted)
                {
                    completedTasks.Add(Tasks[i]);
                    Tasks.Remove(Tasks[i]);
                }
            }

            if (completedTasks.Count == 0)
            {
                SendSnackBarMessage("No completed tasks found");
                return;
            }

            _repository.Delete(completedTasks);
            CalculateProgress();
            SendSnackBarMessage("Completed tasks were removed");
        }

        private void CalculateProgress()
        {
            Task.Run(() =>
            {
                var progressData = _repository.GetTodaysTaskProgress();
                if (progressData.Item1 == 0 || progressData.Item2 == 0)
                {
                    SetProgress(0);
                    return;
                }

                var progress = progressData.Item2 * 100 / progressData.Item1;
                SetProgress(progress);
            });

        }

        private void SetProgress(double progress)
        {
            _eventAggregator.GetEvent<SendMessageToDailyTasks>().Publish((progress, Tasks.Where(x => x.IsCompleted != true).Count()));
        }
    }
}
