using Common.EventAggregators;
using Domain.Models.Tasks;
using Domain.Repository;
using Idoneus.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class TodaysTasksViewModel : BindableBase
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

        private readonly DailyTasksRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        #region Delegates

        private DelegateCommand<TodaysTask> _selectTaskCommand;
        public DelegateCommand<TodaysTask> SelectTaskCommand => _selectTaskCommand ?? (_selectTaskCommand = new DelegateCommand<TodaysTask>(SelectTask));

        private DelegateCommand _deleteCompletedTasksCommand;
        public DelegateCommand DeleteCompletedTasksCommand => _deleteCompletedTasksCommand ?? (_deleteCompletedTasksCommand = new DelegateCommand(DeleteCompletedTasks));

        private DelegateCommand _insertTaskCommand;
        public DelegateCommand InsertTaskCommand => _insertTaskCommand ?? (_insertTaskCommand = new DelegateCommand(InsertTask));

        private DelegateCommand _completeAllCommand;
        public DelegateCommand CompleteAllCommand => _completeAllCommand ?? (_completeAllCommand = new DelegateCommand(CompleteAll));

        #endregion // Delegates

        public TodaysTasksViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _repository = new DailyTasksRepository();
            Tasks = new ObservableCollection<TodaysTask>();
            OnViewTypeChanged();
            CalculateProgress();
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
            _eventAggregator.GetEvent<SendMessageToDailyTasks>().Publish((progress, -1));
        }

        private void SendSnackBarMessage(string message)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(message);
        }

        private void OnViewTypeChanged()
        {
            switch (ViewType)
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
                CheckAndAddRepetetiveTasks();
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

        private void CompleteAll ()
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

        private void InsertTask()
        {
            if (string.IsNullOrEmpty(TaskContent))
            {
                SendSnackBarMessage("Cannot add empty task...");
                return;
            }
            var newTask = new TodaysTask
            {
                ID = Guid.NewGuid().ToString(),
                Content = TaskContent
            };

            var results = _repository.Insert(newTask);

            if (!results) return;
            Tasks.Insert(0, newTask);
            TaskContent = string.Empty;
            CalculateProgress();
            SendSnackBarMessage("Task has been created!");
        }

        private void CheckAndAddRepetetiveTasks()
        {
            if (ViewType == 0) return;

            var rTasks = _repository.GetRepetetiveTasks();
            var failed = false;
            foreach (var task in rTasks)
            {
                if (!task.IsActive) continue;
                var item = Tasks.FirstOrDefault(x => x.RepetetiveTaskID.Equals(task.ID));
                if (item == null)
                {
                    var newTask = new TodaysTask(task);
                    if (!_repository.Insert(newTask))
                    {
                        failed = true;
                        continue;
                    }

                    App.Current.Dispatcher.Invoke(() => Tasks.Insert(0, newTask));
                   
                }
            }

            CalculateProgress();

            if (failed)
            {
                SendSnackBarMessage("Something went wrong while adding repetetive tasks...");
            }
        }

    }
}
