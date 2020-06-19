using Common;
using Common.EventAggregators;
using Domain.Models;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
using Idoneus.Commands;
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
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardViewModel : BindableBase, INavigationAware
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

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly IStorage _storage;
        private readonly ProjectRepository _repository;

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
                    var upcommingTasks = new ObservableCollection<ITask>(_repository.GetUpcommingTasks(DateTime.Now));
                    _eventAggregator.GetEvent<SendMessageEvent<ObservableCollection<Project>>>().Publish(_projects);
                    _eventAggregator.GetEvent<SendMessageToUpcommingTasks<ObservableCollection<ITask>>>().Publish(upcommingTasks);
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

        public void ActivateTab()
        {
            var region = _regionManager.Regions["DashboardProjectRegion"];
            DashboardProjects d = _container.Resolve<DashboardProjects>();
            DashboardUpcommingTasks u = _container.Resolve<DashboardUpcommingTasks>();
            if (ActiveDashboardTab)
            {
                region.Add(d);
                region.Activate(d);
                InitializeData();
            }
            else
            {
                region.Add(u);
                region.Activate(u);
                InitializeData();
            }

        }

       
    }
}
