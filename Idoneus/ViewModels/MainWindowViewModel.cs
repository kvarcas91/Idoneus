using Common;
using Common.EventAggregators;
using Domain.Models;
using Domain.Repository;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Idoneus.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IStorage _storage;
        private readonly IEventAggregator _eventAggregator;

        public MainWindowViewModel(IRegionManager regionManager, IStorage storage, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _storage = storage;
            _eventAggregator = eventAggregator;
            storage.UserName = Environment.UserName;
            UserName = storage.UserName;
            CheckIfUserExist();
            Navigate("Dashboard");
        }

        private DelegateCommand<string> _navigateCommand = null;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));

        private string _title = "Dashboard";
        public string Title
        {
            get => _title;
            set { SetProperty(ref _title, value); }
        }

        private string _userName = "Guest";
        public string UserName
        {
            get => _userName;
            private set { SetProperty(ref _userName, value); }
        }

        private void CheckIfUserExist()
        {
            var repo = new ProjectRepository();
            repo.AlterTable();
            Login();
            
        }

        private void Login()
        {
            var user = new Contributor
            {
                FirstName = "Eduardas",
                LastName = "Slutas",
                Login = UserName
            };
            _eventAggregator.GetEvent <UserLoginMessage<Contributor>>().Publish(user);
          
        }

        private void Navigate(string navigatePath)
        {
            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);

            if (_storage.IsExporting) return;

            Title = navigatePath;
            if (navigatePath.Equals("Dashboard"))
            {
                _storage.FirstLoad = true;
            }
           
            if (navigatePath != null)
            {
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
            }
        }

    }
}
