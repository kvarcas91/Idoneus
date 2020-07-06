using Common;
using Common.EventAggregators;
using DataProcessor.cs;
using Domain.Extentions;
using Domain.Helpers;
using Domain.Models.Project;
using Domain.Repository;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardProjectsViewModel : BindableBase
    {

        #region UI Properties

        private ObservableCollection<Project> _allProjects;
        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        private bool _isExporting = false;
        public bool IsExporting
        {
            get => _isExporting;
            set { SetProperty(ref _isExporting, value); HandleExportParam(value); }
        }

        private string _exportMessage = string.Empty;
        public string ExportMessage
        {
            get => _exportMessage;
            set { SetProperty(ref _exportMessage, value); }
        }

        private List<string> _projectViewType;
        public List<string> ProjectViewType
        {
            get => _projectViewType;
            set { SetProperty(ref _projectViewType, value); }
        }

        private string _selectedProjectViewType = "All";
        public string SelectedProjectViewType
        {
            get => _selectedProjectViewType;
            set { SetProperty(ref _selectedProjectViewType, value); HandleViewTypeSelect(); }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private readonly IStorage _storage;
        private readonly ProjectRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        #region Delegates

        private DelegateCommand _exportCommand;
        public DelegateCommand ExportCommand => _exportCommand ?? (_exportCommand = new DelegateCommand(Export));

        private DelegateCommand _exportJsonCommand;
        public DelegateCommand ExportJsonCommand => _exportJsonCommand ?? (_exportJsonCommand = new DelegateCommand(ExportJson));

        private DelegateCommand<Project> _onItemClickedCommand;
        public DelegateCommand<Project> OnItemClickedCommand => _onItemClickedCommand ?? (_onItemClickedCommand = new DelegateCommand<Project>(OnItemClicked));

        #endregion // Delegates

        #endregion // UI Properties

        public DashboardProjectsViewModel(IEventAggregator eventAggregator, IStorage storage, ProjectRepository repository)
        {
            _storage = storage;
            _repository = repository;
            _eventAggregator = eventAggregator;
            Projects = new ObservableCollection<Project>();
            eventAggregator.GetEvent<SendMessageEvent<ObservableCollection<Project>>>().Subscribe(MessageReceived);
            _projectViewType = new List<string>() { "All", "In Progress", "Completed", "Archived", "Missed" };
        }

        private void HandleExportParam(bool value, bool blockUIThread = true)
        {
            if (blockUIThread) _storage.IsExporting = value;
        }

        private void MessageReceived(ObservableCollection<Project> projects)
        {
            _allProjects = new ObservableCollection<Project>(projects.Clone());
            App.Current.Dispatcher.Invoke(() =>
            {
                Projects.Clear();

                Projects.AddRange(projects);
            });
           
        }

        private void SetExportMessage(string message)
        {
            ExportMessage = message;
        }

        private void HandleViewTypeSelect()
        {
            HandleExportParam(true, false);
            Task.Run(() =>
            {
                if (SelectedProjectViewType.Equals("All"))
                {
                    App.Current.Dispatcher.Invoke(() => Projects = _allProjects.Clone());
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        HandleSearch();
                        return;
                    }

                    return;
                }
                Projects = new ObservableCollection<Project>(_repository.SortByViewType(_allProjects, SelectedProjectViewType));
                if (!string.IsNullOrEmpty(SearchText))
                {
                    HandleSearch();
                    return;
                }

            });
           
        }

        private void HandleSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                HandleViewTypeSelect();
                return;
            }

            Task.Run(() =>
            {
                Enum.TryParse(SelectedProjectViewType.Replace(" ", string.Empty), out ViewType type);

                App.Current.Dispatcher.Invoke(() => Projects.Clear());
                 
                foreach (var item in _allProjects)
                {
                    if (item.HasString(SearchText, type)) App.Current.Dispatcher.Invoke(() => Projects.Add(item));
                }
            });
           
        }

        private void Export()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Save a dashboard file"
            };

            if (dialog.ShowDialog() != true) return;

            var fileName = dialog.FileName;

            IsExporting = true;
            Task.Run(() =>
            {
                var response =  WriteData.Write(fileName, SetExportMessage, Projects);
                IsExporting = false;
                ProcessHelper.Run(fileName);
             
            });
        }

        private void ExportJson()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON Files|*.json",
                Title = "Save a dashboard file"
            };

            if (dialog.ShowDialog() != true) return;

            var fileName = dialog.FileName;

            IsExporting = true;
            Task.Run(() =>
            {
                var response = WriteData.WriteJson(fileName, SetExportMessage, Projects);
                IsExporting = false;
                //ProcessHelper.Run(fileName);

            });
        }

        private void OnItemClicked(Project project)
        {
            var navigationParams = new NavigationParameters
            {
                { "project", project }
            };
            //_regionManager.RequestNavigate("ContentRegion", "Projects", navigationParams);
            _eventAggregator.GetEvent<NavigateRequest<NavigationParameters>>().Publish(("Projects", navigationParams, true));
        }

    }
}
