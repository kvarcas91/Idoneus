using Common.EventAggregators;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class TasksViewModel : BindableBase
    {

        #region Local members

        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectRepository _repository;

        #endregion // Local members

        #region Properties

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { SetProperty(ref _currentProject, value); }
        }

        private ProjectTask _selectedTask;
        public ProjectTask SelectedTask
        {
            get { return _selectedTask; }
            set { SetProperty(ref _selectedTask, value); OnTaskSelectChanged(); }
        }

        #endregion


        public TasksViewModel(IEventAggregator eventAggregator, ProjectRepository repository)
        {
            _eventAggregator = eventAggregator;
            _repository = repository;

            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);
        }

        #region Methods

        #region Tasks

        private void OnTaskSelectChanged()
        {
            if (SelectedTask == null) return;


        }

        #endregion // Tasks

        #region Common

        private void ProjectReceived(Project project)
        {
            CurrentProject = project;
            if (project == null) return;

            Task.Run(() =>
            {
                CurrentProject = _repository.GetProject(project.ID);
            });
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        #endregion Common

        #endregion // Methods

    }
}
