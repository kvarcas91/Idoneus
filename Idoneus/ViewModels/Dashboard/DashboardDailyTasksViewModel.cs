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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Idoneus.ViewModels
{
    public class DashboardDailyTasksViewModel : BindableBase
    {

        private string _repetetiveTaskContent;
        public string RepetetiveTaskContent
        {
            get { return _repetetiveTaskContent; }
            set
            {
                SetProperty(ref _repetetiveTaskContent, value);
            }
        }

        private int _viewType = 0;
        public int ViewType
        {
            get { return _viewType; }
            set
            {
                SetProperty(ref _viewType, value);
                ActivateTab();
            }
        }

        public ObservableCollection<UserControl> Tabs { get; set; }

        private UserControl _activeTab;
        public UserControl ActiveTab
        {
            get { return _activeTab; }
            set { SetProperty(ref _activeTab, value); }
        }

        private double _totalProjectProgress = 0;
        public double TotalProjectProgress
        {
            get { return _totalProjectProgress; }
            set { SetProperty(ref _totalProjectProgress, value); }
        }

        #region Delegates

        // private DelegateCommand _insertRepetetiveTaskCommand;
        //public DelegateCommand InsertRepetetiveTaskCommand => _insertRepetetiveTaskCommand ?? (_insertRepetetiveTaskCommand = new DelegateCommand(InsertRepetetiveTask));

        #endregion // Delegates


        public DashboardDailyTasksViewModel(IEventAggregator eventAggregator)
        {
            Tabs = new ObservableCollection<UserControl>
            {
                new TodaysTasks(),
                new MissedTasks()
            };
            eventAggregator.GetEvent<SendMessageToDailyTasks>().Subscribe(MessageReceived);
        }

        private void MessageReceived(double progress)
        {
            TotalProjectProgress = Math.Round(progress, 0);
            Debug.WriteLine(progress, "PROGRESS");
        }

        //private void InsertRepetetiveTask()
        //{
        //    if (string.IsNullOrEmpty(RepetetiveTaskContent))
        //    {
        //        SendSnackBarMessage("Cannot add empty template...");
        //        return;
        //    }

        //    var repetetiveTask = new RepetetiveTask
        //    {
        //        ID = Guid.NewGuid().ToString(),
        //        Content = RepetetiveTaskContent
        //    };

        //    var insertRepetetiveTaskResult = _repository.Insert(repetetiveTask);

        //   if (!insertRepetetiveTaskResult)
        //    {
        //        SendSnackBarMessage("Something went wrong...");
        //        return;
        //    }

        //    var newTask = new TodaysTask(repetetiveTask);

        //    var results = _repository.Insert(newTask);

        //    if (!results) return;
        //    Tasks.Insert(0, newTask);
        //    TaskContent = string.Empty;
        //    SendSnackBarMessage("Task has been created!");
        //}

        //private void MessageReceived(ObservableCollection<TodaysTask> tasks)
        //{
        //    Tasks = tasks;
        //    CheckAndAddRepetetiveTasks();
        //}

       

        public void ActivateTab()
        {

            switch (ViewType)
            {
                case 0:
                    ActiveTab = Tabs[0];
                    break;
                case 1:
                    ActiveTab = Tabs[1];
                    break;
                default:
                    break;
            }
        }
    }
}
