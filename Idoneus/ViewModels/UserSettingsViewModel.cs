using Common.Enums;
using Common.EventAggregators;
using Common.Settings;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace Idoneus.ViewModels
{
    public class UserSettingsViewModel : BindableBase, INavigationAware
    {

        #region File properties

        public List<string> FileActions { get; set; }

        private string _selectedFileAction = string.Empty;
        public string SelectedFileAction
        {
            get { return _selectedFileAction; }
            set { SetProperty(ref _selectedFileAction, value); }
        }

        #endregion // File properties

        private readonly IEventAggregator _eventAggregator;

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Update));

        public UserSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            FileActions = new List<string>
            {
                "Copy", "Copy and replace", "Move", "Move and replace"
            };
        }

        #region File

        private FileAction GetFileActionSelection()
        {
            try
            {
                FileAction fileAction = (FileAction)Enum.Parse(typeof(FileAction), SelectedFileAction.Replace(" ", ""), true);
                return fileAction;
            }
            catch
            {
                PublishSnackBar("Something went wrong...");
                return FileAction.Default;
            }
        }

        #endregion // File

        #region Common

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        private void Update()
        {
            var fileAction = GetFileActionSelection();
            if (fileAction != FileAction.Default) AppSettings.Instance.FileAction = fileAction;
            AppSettings.Save();
            PublishSnackBar("Changes have been saved!");
        }

        #endregion // Common

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AppSettings.Load();
            var fileAction = AppSettings.Instance.FileAction;
            for (int i = 0; i < FileActions.Count; i++)
            {
                if (FileActions[i].Replace(" ", "").ToLower().EndsWith(fileAction.ToString().ToLower())) SelectedFileAction = FileActions[i];
            }
        }

        #endregion // Navigation

    }
}
