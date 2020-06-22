using Common;
using Common.EventAggregators;
using Common.Settings;
using Domain.Models;
using Domain.Repository;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IStorage _storage;
        private readonly IEventAggregator _eventAggregator;
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly SwatchesProvider _swatchesProvider = new SwatchesProvider();
        private string _currentRegion = string.Empty;

        public MainWindowViewModel(IRegionManager regionManager, IStorage storage, IEventAggregator eventAggregator)
        {
            AppSettings.Load();
            DarkMode = AppSettings.Instance.DarkMode;
            
           if(storage.FirstLoad)  {
                var repository = new ProjectRepository();
                repository.Initialize();
                storage.FirstLoad = false;
            }
           

            _regionManager = regionManager;
            _storage = storage;
            _eventAggregator = eventAggregator;
            storage.UserName = Environment.UserName;
            UserName = storage.UserName;
            Login();
            Navigate("Dashboard");

            _eventAggregator.GetEvent<SendSnackBarMessage>().Subscribe(ReceiveMessage);
            _eventAggregator.GetEvent<NavigateRequest<NavigationParameters>>().Subscribe(ReceiveNavigateRequest);
        }

        private DelegateCommand<string> _navigateCommand = null;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));

        private string _title = "Dashboard";
        public string Title
        {
            get => _title;
            set { SetProperty(ref _title, value); }
        }

        private string _mode = "Light mode";
        public string Mode
        {
            get => _mode;
            set { SetProperty(ref _mode, $"{value} mode"); }
        }

        private string _userName = "Guest";
        public string UserName
        {
            get => _userName;
            private set { SetProperty(ref _userName, value); }
        }

        private bool _darkMode = true;
        public bool DarkMode
        {
            get => _darkMode;
            set { SetProperty(ref _darkMode, value); SetTheme(); }
        }

        private string _snackBarMessage;
        public string SnackBarMessage
        {
            get { return _snackBarMessage; }
            set
            {
                SetProperty(ref _snackBarMessage, value);
            }
        }

        private bool _isSnackBarActive = false;
        public bool IsSnackBarActive
        {
            get { return _isSnackBarActive; }
            set
            {
                SetProperty(ref _isSnackBarActive, value);
            }
        }

        private void SetTheme()
        {
           
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = DarkMode ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
     
            List<string> PrimaryColorsList = _swatchesProvider.Swatches.Select(a => a.Name).ToList();
            Swatch primaryColor = _swatchesProvider.Swatches.FirstOrDefault(a => a.Name == (DarkMode ? "grey" : "blue"));
            Swatch accentColor = _swatchesProvider.Swatches.FirstOrDefault(a => a.Name == (DarkMode ? "orange" : "red"));
            theme.SetPrimaryColor(primaryColor.ExemplarHue.Color);
            theme.SetSecondaryColor(accentColor.AccentExemplarHue.Color);

            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
            Mode = DarkMode ? "Light" : "Dark";
            Task.Run(() =>
            {
                AppSettings.Load();
                AppSettings.Instance.DarkMode = DarkMode;
                AppSettings.Save();
            });
        }

        private void Login()
        {
            var user = new Contributor
            {
                FirstName = "Eduardas",
                LastName = "Slutas"
            };
            _eventAggregator.GetEvent <UserLoginMessage<Contributor>>().Publish(user);
          
        }

        private void ReceiveMessage(string message)
        {
            IsSnackBarActive = true;
            SnackBarMessage = message;
            Task.Run(() => {
                Task.Delay(3000).Wait();
                SnackBarMessage = string.Empty;
                IsSnackBarActive = false;
            });
        }

        private void ReceiveNavigateRequest((string, NavigationParameters) param)
        {
            Navigate(param.Item1, param.Item2);
        }

        private void Navigate(string navigatePath)
        {
            Navigate(navigatePath, null);
        }

        private void Navigate(string navigatePath, NavigationParameters param)
        {
            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);

            if (_storage.IsExporting) return;

            if (_currentRegion.Equals(navigatePath)) return;


            if (navigatePath.Equals("Dashboard"))
            {
                _storage.FirstLoad = true;
            }

            if (navigatePath != null)
            {
                _currentRegion = navigatePath;
                Title = _currentRegion;
                _regionManager.RequestNavigate("ContentRegion", navigatePath, param);
            } 
        }
    }
}
