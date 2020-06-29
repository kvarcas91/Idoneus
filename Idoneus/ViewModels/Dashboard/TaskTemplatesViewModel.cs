using Common.EventAggregators;
using Domain.Models.Tasks;
using Domain.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class TaskTemplatesViewModel : BindableBase
    {

        private ObservableCollection<RepetetiveTask> _tasks;
        private readonly DailyTasksRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<RepetetiveTask> Tasks
        {
            get { return _tasks; }
            private set { SetProperty(ref _tasks, value); }
        }

        private string _templateContent;
        public string TemplateContent
        {
            get { return _templateContent; }
            set
            {
                SetProperty(ref _templateContent, value);
            }
        }

        private DelegateCommand<RepetetiveTask> _selectTemplateCommand;
        public DelegateCommand<RepetetiveTask> SelectTemplateCommand => _selectTemplateCommand ?? (_selectTemplateCommand = new DelegateCommand<RepetetiveTask>(SelectTemplate));

        private DelegateCommand<RepetetiveTask> _deleteTemplateCommand;
        public DelegateCommand<RepetetiveTask> DeleteTemplateCommand => _deleteTemplateCommand ?? (_deleteTemplateCommand = new DelegateCommand<RepetetiveTask>(DeleteTemplate));

        private DelegateCommand _insertTemplateCommand;
        public DelegateCommand InsertTemplateCommand => _insertTemplateCommand ?? (_insertTemplateCommand = new DelegateCommand(InsertTemplate));

        private DelegateCommand<RepetetiveDay> _setDayTemplateCommand;
        public DelegateCommand<RepetetiveDay> SetDayTemplateCommand => _setDayTemplateCommand ?? (_setDayTemplateCommand = new DelegateCommand<RepetetiveDay>(SetDayTemplate));

        public TaskTemplatesViewModel(IEventAggregator eventAggregator)
        {
            Tasks = new ObservableCollection<RepetetiveTask>();
            GetData();
            _repository = new DailyTasksRepository();
            _eventAggregator = eventAggregator;
        }

        private void GetData()
        {
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => Tasks.Clear());
                var tasks = _repository.GetRepetetiveTasks();
                App.Current.Dispatcher.Invoke(() => Tasks.AddRange(tasks));
               
            });
        }

        private void SendSnackBarMessage(string message)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(message);
        }

        private void SelectTemplate(RepetetiveTask task)
        {
            Task.Run(() =>
            {
                var results = _repository.Update(task);
                if (!results) SendSnackBarMessage($"Something went wrong.. {results}");
            });

        }

        private void DeleteTemplate(RepetetiveTask task)
        {
            Task.Run(() =>
            {
                var results = _repository.Delete(task);
                if (results)
                {
                    SendSnackBarMessage("Template has been removed!");
                    App.Current.Dispatcher.Invoke(() => Tasks.Remove(task));
                   
                    return;
                }
                SendSnackBarMessage("Something went wrong...");
            });

        }

        private void SetDayTemplate(RepetetiveDay repetetiveDay)
        {
            Task.Run(() =>
            {
                var parentTask = Tasks.Where(t => t.ID.Equals(repetetiveDay.ParentID)).FirstOrDefault();
                if (parentTask == null) return;

                parentTask.SetDate();
                _repository.Update(parentTask);
            });
        }

        private void InsertTemplate()
        {
            if (string.IsNullOrEmpty(TemplateContent))
            {
                SendSnackBarMessage("Cannot add empty template...");
                return;
            }
            var newTemplate = new RepetetiveTask
            {
                ID = Guid.NewGuid().ToString(),
                Content = TemplateContent,
                IsActive = false
            };

            var results = _repository.Insert(newTemplate, "repetetive_tasks");


            if (!results) return;
           
            TemplateContent = string.Empty;
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => Tasks.Add(newTemplate));
              
                SendSnackBarMessage("Template has been created!");
            });
           
        }
    }
}
