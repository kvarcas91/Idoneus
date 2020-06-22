using Common.EventAggregators;
using Idoneus.Views;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
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

        private int _missedTaskCount = 0;
        public int MissedTaskCount
        {
            get { return _missedTaskCount; }
            set { SetProperty(ref _missedTaskCount, value); }
        }

        public DashboardDailyTasksViewModel(IEventAggregator eventAggregator)
        {

            Tabs = new ObservableCollection<UserControl>
            {
                new TodaysTasks(),
                new MissedTasks(),
                new TaskTemplates()
            };
            eventAggregator.GetEvent<SendMessageToDailyTasks>().Subscribe(MessageReceived);
        }

        private void MessageReceived((double progress, int missedCount) param)
        {
            
            if (param.progress != -1) TotalProjectProgress = Math.Round(param.progress, 0);
            if (param.missedCount != -1) MissedTaskCount = param.missedCount;
        }

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
