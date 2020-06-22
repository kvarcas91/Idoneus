using Common;
using Common.EventAggregators;
using Domain.Models;
using Domain.Models.Base;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
using Idoneus.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Idoneus.ViewModels
{
    public class DashboardViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {

        private double _totalProjectProgress = 0D;
        public double TotalProjectProgress
        {
            get => _totalProjectProgress;
            set { SetProperty(ref _totalProjectProgress, value); }
        }

        private bool _isDataLoaded = false;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set { SetProperty(ref _isDataLoaded, value); }
        }

       

        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

        private bool _activeDashboardTab = true;
        public bool ActiveDashboardTab
        {
            get => _activeDashboardTab;
            set
            {
                if (_storage.IsExporting) return;
                SetProperty(ref _activeDashboardTab, value); ActivateTab(); }
        }

        public bool KeepAlive => true;

        private UserControl _activeTab;

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly IStorage _storage;
        private IRegion _region;
        private readonly ProjectRepository _repository;
        private DashboardProjects d;
        private DashboardUpcommingTasks u;

        public DashboardViewModel(IEventAggregator eventAggregator, 
            IRegionManager regionManager, IContainerExtension container, IStorage storage)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _container = container;
            _storage = storage;
          
            _repository = new ProjectRepository();
            _eventAggregator.GetEvent<UserLoginMessage<Contributor>>().Subscribe(GetUser);

        }

        #region Navigation

        private void GetUser(Contributor user)
        {
            Debug.WriteLine(user, "user");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _region.Deactivate(_activeTab);
            navigationContext.NavigationService.Region.RegionManager.Regions.Remove("DashboardTodayTaskRegion");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
            InitializeData();
                
        }

        #endregion // Navigation

        public void InitializeData()
        {
            IsDataLoaded = _storage.FirstLoad ? false : true;
            var response = _repository.Initialize();
            Debug.WriteLine(response);
            Task.Run(() =>
            {
                if (response.Success)
                {
                    _projects = new ObservableCollection<Project>(_repository.GetProjects());
                    var upcommingTasks = new ObservableCollection<ITask>(_repository.GetUpcommingTasks(DateTime.Now.AddDays(7)));
                    //var todaysTasks = new ObservableCollection<TodaysTask>(_repository.GetTodaysTasks(0));

                    _eventAggregator.GetEvent<SendMessageEvent<ObservableCollection<Project>>>().Publish(_projects);
                    _eventAggregator.GetEvent<SendMessageToUpcommingTasks<ObservableCollection<ITask>>>().Publish(upcommingTasks);
                    //_eventAggregator.GetEvent<SendMessageToDailyTasks<ObservableCollection<TodaysTask>>>().Publish(todaysTasks);
                    SetTotalProgress();
                }
                _storage.FirstLoad = false;
                IsDataLoaded = true;
            });
        }

        private void SetTotalProgress()
        {
            if (_projects == null || _projects.Count == 0) return;

            TotalProjectProgress = 0;

            foreach (var item in _projects)
            {
                TotalProjectProgress +=  item.GetProgress() / _projects.Count;
            }

            TotalProjectProgress = Math.Round(TotalProjectProgress, 0);
        }

        public void InitRegion()
        {
            _region = _regionManager.Regions["DashboardProjectRegion"];
            d = _container.Resolve<DashboardProjects>();
            u = _container.Resolve<DashboardUpcommingTasks>();
            _region.Add(d);
            _region.Add(u);
            _region.Activate(d);

            _activeTab = d;
            InitializeData();
        }

        public void ActivateTab()
        {

            if (ActiveDashboardTab)
            {
                _activeTab = d;
                _region.Activate(d);
                _region.Deactivate(u);
                InitializeData();
            }
            else
            {
                _activeTab = u;
                _region.Activate(u);
                _region.Deactivate(d);
                InitializeData();
            }
        }
    }
}
