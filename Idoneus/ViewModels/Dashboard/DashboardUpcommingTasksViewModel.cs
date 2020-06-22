using Common;
using Common.EventAggregators;
using Domain.Extentions;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardUpcommingTasksViewModel : BindableBase
    {

        private ObservableCollection<ITask> _allTasks;
        public ObservableCollection<ITask> _upcomingTasks;
        private readonly ProjectRepository _repository;
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

        public DashboardUpcommingTasksViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SendMessageToUpcommingTasks<ObservableCollection<ITask>>>().Subscribe(MessageReceived);
            _repository = new ProjectRepository();
        }

        private void MessageReceived(ObservableCollection<ITask> projects)
        {
            _allTasks = projects.Clone();
            UpcomingTasks = projects;
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
                    App.Current.Dispatcher.Invoke(() => UpcomingTasks = _allTasks.Clone());
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
        }

    }
}
