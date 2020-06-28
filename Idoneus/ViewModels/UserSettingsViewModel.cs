using Common.Enums;
using Common.EventAggregators;
using Common.Settings;
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
            set { SetProperty(ref _selectedFileAction, value); SetFileActionSelection(); }
        }

        #endregion // File properties

        private readonly IEventAggregator _eventAggregator;

        public UserSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            FileActions = new List<string>
            {
                "Copy", "Copy and replace", "Move", "Move and replace"
            };
        }

        #region File

        private void SetFileActionSelection()
        {
            try
            {
                FileAction fileAction = (FileAction)Enum.Parse(typeof(FileAction), SelectedFileAction.Replace(" ", ""), true);
                AppSettings.Instance.FileAction = fileAction;
                Update();
            }
            catch
            {
                PublishSnackBar("Something went wrong...");
                return;
            }


            Update();

        }

        #endregion // File

        #region Common

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        private void Update()
        {
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
