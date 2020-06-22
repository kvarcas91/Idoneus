using Common.EventAggregators;
using Domain.Models;
using Domain.Models.Comments;
using Domain.Models.Project;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DetailsViewModel : BindableBase
    {

        private Project _currentProject;

        private int _descriptionViewType = 0;
        public int DescriptionViewType
        {
            get { return _descriptionViewType; }
            set { SetProperty(ref _descriptionViewType, value); }
        }

        private int _selectedContributorCount = 0;
        public int SelectedContributorCount
        {
            get { return _selectedContributorCount; }
            set { SetProperty(ref _selectedContributorCount, value); }
        }

        private string _projectDescription;
        public string ProjectDescription
        {
            get { return _projectDescription; }
            set { SetProperty(ref _projectDescription, value); }
        }

        private readonly IEventAggregator _eventAggregator;

        #region Observable Collections

        private ObservableCollection<Comment> _comments;
        public ObservableCollection<Comment> Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }

        private ObservableCollection<Link> _links;
        public ObservableCollection<Link> Links
        {
            get { return _links; }
            set { SetProperty(ref _links, value); }
        }

        private readonly ProjectRepository _repository;
        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors
        {
            get { return _contributors; }
            set { SetProperty(ref _contributors, value); }
        }

        private ObservableCollection<Contributor> _selectedContributors;
        public ObservableCollection<Contributor> SelectedContributors
        {
            get { return _selectedContributors; }
            set { SetProperty(ref _selectedContributors, value); }
        }

        #endregion // ObservableCollections

        private Action _deselectContributors;

        #region Delegates

        private DelegateCommand _deleteSelectedContributorsCommand;
        public DelegateCommand DeleteSelectedContributorsCommand => _deleteSelectedContributorsCommand ?? (_deleteSelectedContributorsCommand = new DelegateCommand(DeleteSelectedContributors));

        private DelegateCommand _unselectContributorsCommand;
        public DelegateCommand UnselectContributorsCommand => _unselectContributorsCommand ?? (_unselectContributorsCommand = new DelegateCommand(UnselectContributors));

        #endregion // Delegates

        public DetailsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Contributors = new ObservableCollection<Contributor>();
            SelectedContributors = new ObservableCollection<Contributor>();
            Comments = new ObservableCollection<Comment>();
            Links = new ObservableCollection<Link>();
            _repository = new ProjectRepository();

            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);
        }


        private void ProjectReceived(Project project)
        {
            _currentProject = project;
            if (project == null) return;

            Contributors.AddRange(_currentProject.Contributors);
            ProjectDescription = project.Content;
            SeparateComments();
        }

        private void SeparateComments()
        {
            Comments.Clear();
            Links.Clear();
            foreach (var item in _currentProject.Comments)
            {
                if (item is Comment comment) Comments.Add(comment);
                if (item is Link link) Links.Add(link);
            }
        }

        public void SetSelectedContributors(IEnumerable<Contributor> list)
        {

            SelectedContributors.Clear();
            SelectedContributors.AddRange(list);
            SelectedContributorCount = SelectedContributors.Count;
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        private void DeleteSelectedContributors()
        {
            Task.Run(() =>
            {
                _repository.UnassignContributors(SelectedContributors, _currentProject.ID);

                for (int i = SelectedContributors.Count - 1; i >= 0; i--)
                {
                    App.Current.Dispatcher.Invoke(() => Contributors.Remove(SelectedContributors[i]));
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    SelectedContributors.Clear();
                    SelectedContributorCount = SelectedContributors.Count;
                    PublishSnackBar("Contributors were removed!");
                });
            });
        }

        private void UnselectContributors()
        {
            _deselectContributors();
            SelectedContributors.Clear();
            SelectedContributorCount = SelectedContributors.Count;
        }

        public void SetDeselectAction(Action action)
        {
            _deselectContributors = action;
        }


    }
}
