using Common;
using Common.EventAggregators;
using Common.Settings;
using Domain.Extentions;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Project;
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
        private readonly ProjectRepository _repository;
        private readonly IRegionManager _regionManager;
        private readonly IStorage _storage;
        private readonly IEventAggregator _eventAggregator;
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly SwatchesProvider _swatchesProvider = new SwatchesProvider();
        private string _currentRegion = string.Empty;
        private Project _editableProject;

        public MainWindowViewModel(IRegionManager regionManager, IStorage storage, IEventAggregator eventAggregator, ProjectRepository repository)
        {
            AppSettings.Load();
            _repository = repository;
            DarkMode = AppSettings.Instance.DarkMode;

            if (storage.FirstLoad)  {

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
            _eventAggregator.GetEvent<EditProjectRequest<Project>>().Subscribe(EditProject);
            _eventAggregator.GetEvent<NavigateRequest<NavigationParameters>>().Subscribe(ReceiveNavigateRequest);
        }

        private DelegateCommand<string> _navigateCommand = null;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));

        private DelegateCommand _addNewProjectCommand = null;
        public DelegateCommand AddNewProjectCommand => _addNewProjectCommand ?? (_addNewProjectCommand = new DelegateCommand(AddNewProject));

        private DelegateCommand _closeDialogCommand = null;
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

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

        #region New Project

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _priority;
        public string Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        private DateTime _dueDate = DateTime.Now.AddDays(7);
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        #endregion // New Project

        private void CloseDialog()
        {
            var dialog = DialogHost.CloseDialogCommand;
            dialog.Execute(null, null);

            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);
            ClearProjectData();
        }

        private void EditProject(Project project)
        {
            DueDate = project.DueDate;
            Priority = project.Priority.ToString();
            Content = project.Content;
            Header = project.Header;

            _editableProject = project;

            var dialog = DialogHost.OpenDialogCommand;
            dialog.Execute(null, null);
        }

        private void AddNewProject()
        {
            if (!IsNewProjectValid()) return;

            Priority priority = (Priority)Enum.Parse(typeof(Priority), Priority);

            var ID = _editableProject == null ? Guid.NewGuid().ToString() : _editableProject.ID;
            var submitionDate = _editableProject == null ? DateTime.Now : _editableProject.SubmitionDate;

            Project project;
            if (_editableProject == null)
            {
                project = new Project
                {
                    ID = ID,
                    SubmitionDate = submitionDate,
                    Header = Header,
                    Content = Content,
                    DueDate = DueDate,
                    Priority = priority
                };
            }
            else
            {
                project = _editableProject;
                project.Header = Header;
                project.Content = Content;
                project.DueDate = DueDate;
                project.Priority = priority;
            }
            

            if (_editableProject == null) _repository.Insert(project, "projects");
            else _repository.Update(project);


            FileHelper.InitializeProjectFolder(project.ID);

            CloseDialog();

            if (_currentRegion.Equals("Projects"))
            {
                _eventAggregator.GetEvent<NotifyProjectChanged<Project>>().Publish(project);
            }
            else
            {
                var navigationParams = new NavigationParameters
                    {
                        { "project", project.Clone() }
                    };
                Navigate("Projects", navigationParams);
            }

            _editableProject = null;
        }

        private bool IsNewProjectValid()
        {
            if (!ValidationHelper.Validate(Priority, Header,Content) || DueDate < DateTime.Now)
            {
                return false;
            }

            return true;
        }

        private void ClearProjectData()
        {
            Header = string.Empty;
            Content = string.Empty;
            DueDate = DateTime.Now.AddDays(7);
            Priority = string.Empty;
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

        private void ReceiveNavigateRequest((string, NavigationParameters, bool) param)
        {
            var drawer = DrawerHost.CloseDrawerCommand;
            drawer.Execute(null, null);
            Title = _currentRegion;

            if (param.Item3) Navigate(param.Item1, param.Item2);
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

            if (navigatePath != null)
            {
                _currentRegion = navigatePath;
                Title = _currentRegion;
                _regionManager.RequestNavigate("ContentRegion", navigatePath, param);
            } 
        }
    }
}
