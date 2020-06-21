using Common.EventAggregators;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardDailyTasksViewModel : BindableBase
    {

        private ObservableCollection<TodaysTask> _tasks;
        public ObservableCollection<TodaysTask> Tasks
        {
            get { return _tasks; }
            private set { SetProperty(ref _tasks, value); }
        }

        private int _viewType = 1;
        public int ViewType
        {
            get { return _viewType; }
            set
            {
                SetProperty(ref _viewType, value);
                OnViewTypeChanged();
            }
        }

        private string _taskContent;
        public string TaskContent
        {
            get { return _taskContent; }
            set
            {
                SetProperty(ref _taskContent, value);
            }
        }

        private DelegateCommand<TodaysTask> _selectTaskCommand;
        public DelegateCommand<TodaysTask> SelectTaskCommand => _selectTaskCommand ?? (_selectTaskCommand = new DelegateCommand<TodaysTask>(SelectTask));

        private DelegateCommand _deleteCompletedTasksCommand;
        public DelegateCommand DeleteCompletedTasksCommand => _deleteCompletedTasksCommand ?? (_deleteCompletedTasksCommand = new DelegateCommand(DeleteCompletedTasks));

        private DelegateCommand<string> _insertTaskCommand;
        public DelegateCommand<string> InsertTaskCommand => _insertTaskCommand ?? (_insertTaskCommand = new DelegateCommand<string>(InsertTask));

        private readonly ProjectRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public DashboardDailyTasksViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<SendMessageToDailyTasks<ObservableCollection<TodaysTask>>>().Subscribe(MessageReceived);
            _repository = new ProjectRepository();
            
        }

        private void SendSnackBarMessage(string message)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(message);
        }

        private void OnViewTypeChanged()
        {
            switch(ViewType)
            {
                case 0:
                    GetTasks(-1);
                    break;
                case 1:
                    GetTasks(0);
                    break;
                case 2:
                    GetTasks(-2);
                    break;
            }
        }

        private void GetTasks(int days)
        {
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => Tasks.Clear());
                var tasks = _repository.GetTodaysTasks(days);
                App.Current.Dispatcher.Invoke(() => Tasks.AddRange(tasks));
            });
           
           
        }

        private void SelectTask(TodaysTask task)
        {
            var index = Tasks.IndexOf(task);

            // If task is completed - move it to the end. Otherwise, move to front
            var newIndex = task.IsCompleted ? Tasks.Count - 1 : 0;

            //DBHelper.UpdateTodaysTask(task);
            var results = _repository.Update(task);
            if (results) Tasks.Move(index, newIndex);
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
            SendSnackBarMessage("Completed tasks were removed");
        }

        private void InsertTask(string task)
        {
            if (string.IsNullOrEmpty(task))
            {
                SendSnackBarMessage("Cannot add empty task...");
                return;
            }
            var newTask = new TodaysTask
            {
                ID = Guid.NewGuid().ToString(),
                Content = task
            };

            var results = _repository.Insert(newTask);

            if (!results) return;
            Tasks.Insert(0, newTask);
            TaskContent = string.Empty;
            SendSnackBarMessage("Task has been created!");
        }

        private void MessageReceived(ObservableCollection<TodaysTask> tasks)
        {
            Tasks = tasks;
        }

    }
}
