﻿using Common;
using Common.EventAggregators;
using DataProcessor.cs;
using Domain.Extentions;
using Domain.Models.Project;
using Domain.Repository;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
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

        private DelegateCommand _exportCommand;
        private readonly IStorage _storage;
        private readonly ProjectRepository _repository;

        public DelegateCommand ExportCommand => _exportCommand ?? (_exportCommand = new DelegateCommand(Export));

        #endregion // UI Properties

        public DashboardProjectsViewModel(IEventAggregator eventAggregator, IStorage storage)
        {
            _storage = storage;
            _repository = new ProjectRepository();
            eventAggregator.GetEvent<SendMessageEvent<ObservableCollection<Project>>>().Subscribe(MessageReceived);
            _projectViewType = new List<string>() { "All", "In Progress", "Completed", "Archived", "Delayed" };
        }

        private void HandleExportParam(bool value, bool blockUIThread = true)
        {
            if (blockUIThread) _storage.IsExporting = value;
        }

        private void MessageReceived(ObservableCollection<Project> projects)
        {
            
            _allProjects = projects.Clone();
            Projects = projects;
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
                Process.Start(fileName);
            });
        }
    }
}