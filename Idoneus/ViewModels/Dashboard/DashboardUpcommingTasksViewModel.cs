﻿using Common.EventAggregators;
using Domain.Extentions;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardUpcommingTasksViewModel : BindableBase
    {

        private ObservableCollection<ITask> _allTasks;
        public ObservableCollection<ITask> _upcomingTasks;
        private readonly ProjectRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ITask> UpcomingTasks
        {
            get { return _upcomingTasks; }
            set { SetProperty(ref _upcomingTasks, value); }
        }

   
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { SetProperty(ref _selectedDate, value); HandleDateChange(); }
        }

        private DelegateCommand<ITask> _onItemClickedCommand;
        public DelegateCommand<ITask> OnItemClickedCommand => _onItemClickedCommand ?? (_onItemClickedCommand = new DelegateCommand<ITask>(OnItemClicked));

        public DashboardUpcommingTasksViewModel(IEventAggregator eventAggregator, ProjectRepository repository)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<SendMessageToUpcommingTasks<ObservableCollection<ITask>>>().Subscribe(MessageReceived);
            _repository = repository;
        }

        private void MessageReceived(ObservableCollection<ITask> tasks)
        {
           
           _allTasks = new ObservableCollection<ITask>(tasks.Clone());
            UpcomingTasks = tasks;
        }

        private void HandleDateChange()
        {
            Task.Run(() =>
            {
                if (_allTasks == null) _allTasks = new ObservableCollection<ITask>();
                App.Current.Dispatcher.Invoke(() => 
                {
                    _allTasks.Clear();
                    _allTasks.AddRange(_repository.GetUpcommingTasks(SelectedDate));
                    UpcomingTasks = _allTasks.Clone();
                });
               
                HandleSearch();
            });
           
        }

        private void HandleSearch()
        {
            Task.Run(() =>
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    App.Current.Dispatcher.Invoke(() => UpcomingTasks = new ObservableCollection<ITask>(_allTasks.Clone()));
                    return;
                }
                App.Current.Dispatcher.Invoke(() => UpcomingTasks.Clear());

                foreach (var item in _allTasks)
                {
                    if (item.HasString(SearchText)) App.Current.Dispatcher.Invoke(() => UpcomingTasks.Add(item));
                }
            });

        }

        private void OnItemClicked(ITask task)
        {
            var parentProject = _repository.GetParentProject(task);
            var navigationParams = new NavigationParameters
            {
                { "project", parentProject }
            };

            _eventAggregator.GetEvent<NavigateRequest<NavigationParameters>>().Publish(("Projects", navigationParams, true));
        }

    }
}
