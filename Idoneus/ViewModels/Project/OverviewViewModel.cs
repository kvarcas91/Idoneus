﻿using Common;
using Common.EventAggregators;
using Domain.Models.Project;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class OverviewViewModel : BindableBase
    {

        #region Local members

        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectRepository _repository;

        #endregion // Local members

        #region Properties

        #region Project info

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { SetProperty(ref _currentProject, value); }
        }

        private Status _projectStatus;
        public Status ProjectStatus
        {
            get { return _projectStatus; }
            set { SetProperty(ref _projectStatus, value); }
        }

        #endregion // Project info

        #endregion // Properties

        #region Delegates

        #region Project

        private DelegateCommand _archiveProjectCommand;
        public DelegateCommand ArchiveProjectCommand => _archiveProjectCommand ?? (_archiveProjectCommand = new DelegateCommand(ArchiveProject));

        private DelegateCommand _editProjectCommand;
        public DelegateCommand EditProjectCommand => _editProjectCommand ?? (_editProjectCommand = new DelegateCommand(EditProject));

        private DelegateCommand _deleteProjectCommand;
        public DelegateCommand DeleteProjectCommand => _deleteProjectCommand ?? (_deleteProjectCommand = new DelegateCommand(DeleteProject));

        #endregion // Project

        #endregion // Delegates

        public OverviewViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _repository = new ProjectRepository();
            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);
        }

        #region Methods

        #region Project

        private void EditProject()
        {
            if (CurrentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

        }

        private void ArchiveProject()
        {
            if (CurrentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            ProjectStatus = CurrentProject.Status == Status.Archived ? Status.InProgress : Status.Archived;
            CurrentProject.Status = ProjectStatus;
            Task.Run(() => 
            {
                if (_repository.Update(CurrentProject))
                {
                    PublishSnackBar("Update successfully!");
                    return;
                }

                PublishSnackBar("Something went wrong...");
            });
        }

        private void DeleteProject()
        {
            if (CurrentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            Task.Run(() =>
            {
                _repository.DeleteProject(CurrentProject);
                CurrentProject = null;

                App.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<NavigateRequest<NavigationParameters>>().Publish(("Dashboard", null)));
            });

           
        }

        #endregion // Project

        #region Common

        private void ProjectReceived(Project project)
        {
            CurrentProject = project;
            if (project == null) return;

            Task.Run(() =>
            {
                CurrentProject = _repository.GetProject(project.ID);
                ProjectStatus = CurrentProject.Status;

            });
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        #endregion // Common

        #endregion // Methods

    }
}
