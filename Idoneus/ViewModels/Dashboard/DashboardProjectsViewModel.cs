using Common;
using Common.EventAggregators;
using DataProcessor.cs;
using Domain.Models.Project;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DashboardProjectsViewModel : BindableBase
    {
        public ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        private bool _isExporting = false;
        public bool IsExporting
        {
            get => _isExporting;
            set { SetProperty(ref _isExporting, value); _storage.IsExporting = value; }
        }

        private string _exportMessage = string.Empty;
        public string ExportMessage
        {
            get => _exportMessage;
            set { SetProperty(ref _exportMessage, value); }
        }

        private DelegateCommand _exportCommand;
        private readonly IStorage _storage;

        public DelegateCommand ExportCommand => _exportCommand ?? (_exportCommand = new DelegateCommand(Export));

        public DashboardProjectsViewModel(IEventAggregator eventAggregator, IStorage storage)
        {
            _storage = storage;
            eventAggregator.GetEvent<SendMessageEvent<ObservableCollection<Project>>>().Subscribe(MessageReceived);
        }

        private void MessageReceived(ObservableCollection<Project> projects)
        {
            Projects = projects;
            foreach (var item in projects)
            {
                Debug.WriteLine(item);
            }
           
        }

        private void SetExportMessage(string message)
        {
            ExportMessage = message;
        }

        private void Export()
        {
            IsExporting = true;
            Task.Run(() =>
            {
                WriteData.Write("somewhere", SetExportMessage);
                IsExporting = false;
            });
        }
    }
}
