using Common.Enums;
using Common.EventAggregators;
using Common.Settings;
using Domain.Data;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Comments;
using Domain.Models.Project;
using Domain.Repository;
using Idoneus.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Idoneus.ViewModels
{
    public class DetailsViewModel : BindableBase, IFileDragDropTarget
    {

        #region Local members

        private Project _currentProject;
        private string _basePath = string.Empty;

        #endregion // Local members

        #region Properties

        #region Description / Contributors

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

        private string _selectedContributor;
        public string SelectedContributor
        {
            get { return _selectedContributor; }
            set { SetProperty(ref _selectedContributor, value); }
        }

        #endregion // Description / Contributors

        #region Files

        private string _currentPath = string.Empty;
        public string CurrentPath
        {
            get { return _currentPath; }
            set { SetProperty(ref _currentPath, value); }
        }

        private string _folderName = string.Empty;
        public string FolderName
        {
            get { return _folderName; }
            set { SetProperty(ref _folderName, value); }
        }

        private bool _isAddFolderPanelVisible = false;
        public bool IsAddFolderPanelVisible
        {
            get { return _isAddFolderPanelVisible; }
            set { SetProperty(ref _isAddFolderPanelVisible, value); }
        }

        private ProjectFolder _currentVersion;
        public ProjectFolder CurrentVersion
        {
            get { return _currentVersion; }
            set { SetProperty(ref _currentVersion, value); if (_currentVersion != null) SetVersion(); }
        }

        #endregion // Files

        #endregion // Properties

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

        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors
        {
            get { return _contributors; }
            set { SetProperty(ref _contributors, value); }
        }

        public ObservableCollection<Contributor> AllContributors { get; set; }
        public ObservableCollection<ProjectFolder> DataVersions { get; set; }

        private ObservableCollection<Contributor> _selectedContributors;
        public ObservableCollection<Contributor> SelectedContributors
        {
            get { return _selectedContributors; }
            set { SetProperty(ref _selectedContributors, value); }
        }

        private ObservableCollection<IData> _relatedFiles;
        public ObservableCollection<IData> RelatedFiles
        {
            get { return _relatedFiles; }
            set { SetProperty(ref _relatedFiles, value); }
        }

        #endregion // ObservableCollections

        #region Loaders

        private bool _isFileDataLoading = false;
        public bool IsFileDataLoading
        {
            get { return _isFileDataLoading; }
            set { SetProperty(ref _isFileDataLoading, value); }
        }

        private string _loadingMessage = "Loading...";
        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set { SetProperty(ref _loadingMessage, value); }
        }

        private int _maxLoaderValue = 0;
        public int MaxLoaderValue
        {
            get { return _maxLoaderValue; }
            set { SetProperty(ref _maxLoaderValue, value); }
        }

        private int _currentLoaderValue = 0;
        public int CurrentLoaderValue
        {
            get { return _currentLoaderValue; }
            set { SetProperty(ref _currentLoaderValue, value); }
        }

        #endregion // Loaders

        #region Delegates

        private DelegateCommand _deleteSelectedContributorsCommand;
        public DelegateCommand DeleteSelectedContributorsCommand => _deleteSelectedContributorsCommand ?? (_deleteSelectedContributorsCommand = new DelegateCommand(DeleteSelectedContributors));

        private DelegateCommand _unselectContributorsCommand;
        public DelegateCommand UnselectContributorsCommand => _unselectContributorsCommand ?? (_unselectContributorsCommand = new DelegateCommand(UnselectContributors));

        private DelegateCommand _addContributorCommand;
        public DelegateCommand AddContributorCommand => _addContributorCommand ?? (_addContributorCommand = new DelegateCommand(AddContributor));

        private DelegateCommand _addNewVersionCommand;
        public DelegateCommand AddNewVersionCommand => _addNewVersionCommand ?? (_addNewVersionCommand = new DelegateCommand(AddNewVersion));

        private DelegateCommand _navigateBackCommand;
        public DelegateCommand NavigateBackCommand => _navigateBackCommand ?? (_navigateBackCommand = new DelegateCommand(NavigateBack));

        private DelegateCommand _homeCommand;
        public DelegateCommand HomeCommand => _homeCommand ?? (_homeCommand = new DelegateCommand(Home));

        private DelegateCommand _showAddFolderPanelCommand;
        public DelegateCommand ShowAddFolderPanelCommand => _showAddFolderPanelCommand ?? (_showAddFolderPanelCommand = new DelegateCommand(ShowAddFolderPanel));

        private DelegateCommand _addFolderCommand;
        public DelegateCommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new DelegateCommand(AddFolder));

        private DelegateCommand _removeVersionCommand;
        public DelegateCommand RemoveVersionCommand => _removeVersionCommand ?? (_removeVersionCommand = new DelegateCommand(RemoveVersion));

        private DelegateCommand<IData> _fileListItemClickCommand;
        public DelegateCommand<IData> FileListItemClickCommand => _fileListItemClickCommand ?? (_fileListItemClickCommand = new DelegateCommand<IData>(FileListItemClick));

        private DelegateCommand<IData> _howInExplorerCommand;
        public DelegateCommand<IData> ShowInExplorerCommand => _howInExplorerCommand ?? (_howInExplorerCommand = new DelegateCommand<IData>(ShowInExplorer));

        private DelegateCommand<IData> _deleteFileCommand;
        public DelegateCommand<IData> DeleteFileCommand => _deleteFileCommand ?? (_deleteFileCommand = new DelegateCommand<IData>(DeleteFile));

        #endregion // Delegates

        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectRepository _repository;
        private Action _deselectContributors;

        public DetailsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Contributors = new ObservableCollection<Contributor>();
            SelectedContributors = new ObservableCollection<Contributor>();
            Comments = new ObservableCollection<Comment>();
            Links = new ObservableCollection<Link>();
            _repository = new ProjectRepository();

            AllContributors = new ObservableCollection<Contributor>(_repository.GetAllContributors());
            DataVersions = new ObservableCollection<ProjectFolder>();
            RelatedFiles = new ObservableCollection<IData>();

            eventAggregator.GetEvent<SendCurrentProject<Project>>().Subscribe(ProjectReceived);
        }

        #region Files

        private void DeleteFile(IData data)
        {
            var response = data.Delete();
            if (!response.Success)
            {
                PublishSnackBar($"Failed to delete: {response.Message}");
                return;
            }

            RelatedFiles.Remove(data);
            PublishSnackBar($"Deleted successfully!");
        }

        private void ShowInExplorer(IData data)
        {
            try
            {
                if (data is ProjectFile)
            {
                ProcessHelper.Run(FileHelper.GetFullParentPath(data.Path));
                return;
            }
           
                ProcessHelper.Run(FileHelper.GetExecutablePath(data.Path));
            }
            catch(Exception e)
            {
                PublishSnackBar(e.Message);
            }
           
        }

        private void RemoveVersion()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            if (DataVersions.Count < 2)
            {
                PublishSnackBar("At least one version must be present");
                return;
            }

            var version = CurrentVersion.ToString();
            Task.Run(() =>
            {
                var response = CurrentVersion.Delete();
                if (!response.Success)
                {
                    PublishSnackBar($"Failed to remove version: {response.Message}");
                    return;
                }
                App.Current.Dispatcher.Invoke(() => DataVersions.Remove(CurrentVersion));

                CurrentPath = Path.Combine(_basePath, CurrentVersion.ToString());
            });
        }

        private void FileListItemClick(IData data)
        {
            if (data is ProjectFolder folder)
            {
                CurrentPath = Path.Combine(CurrentPath, folder.Name);

                Task.Run(() => {
                    SetFiles();
                });
                return;
            }
            try
            {
                ProcessHelper.Run(FileHelper.GetExecutablePath(data.Path));
            }
            catch (Exception e)
            {
                PublishSnackBar(e.Message);
            }
          
        }

        private void AddFolder()
        {
            if (string.IsNullOrEmpty(FolderName))
            {
                PublishSnackBar("Folder name cannot be emty");
                return;
            }

            var newPath = Path.Combine(CurrentPath, FolderName);

            try
            {
                FileHelper.CreateFolderIfNotExist(newPath);
            }
            catch (Exception e)
            {
                PublishSnackBar($"Cannot create folder due to: {e.Message}");
                return;
            }

            RelatedFiles.Add(new ProjectFolder(newPath));
            FolderName = string.Empty;
            IsAddFolderPanelVisible = false;
        }

        private void ShowAddFolderPanel()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            IsAddFolderPanelVisible = !IsAddFolderPanelVisible;
        }

        private void Home()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            var homePath = Path.Combine(_basePath, CurrentVersion.ToString());
            if (CurrentPath.Equals(homePath)) return;

            CurrentPath = homePath;
            Task.Run(() =>
            {
                SetFiles();
            });
        }

        private void NavigateBack()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            if (!FileHelper.CanNavigateBack(CurrentPath, Path.Combine(_basePath, CurrentVersion.ToString()))) return;
            CurrentPath = FileHelper.GetParentPath(CurrentPath);

            Task.Run(() =>
            {
                SetFiles();
            });
        }

        private void SetFiles()
        {
        
            if (RelatedFiles != null)
            {
                App.Current.Dispatcher.Invoke(() => RelatedFiles.Clear());
            }

            foreach (var item in FileHelper.GetFolderContent(CurrentPath))
            {
                App.Current.Dispatcher.Invoke(() => RelatedFiles.Add(item));
            }

        }

        private void SetVersion()
        {
            CurrentPath = Path.Combine(_basePath, CurrentVersion.ToString());
            Task.Run(() =>
            {
                SetFiles();
            });

        }

        private void AddNewVersion()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            if (IsFileDataLoading) return;
            int version;
            try
            {
                version = Convert.ToInt32(DataVersions[DataVersions.Count - 1].Name.Substring(1));
            }
            catch (Exception e)
            {
                PublishSnackBar($"Somethinh went wrong.. {e.Message}");
                return;
            }

            var versionName = $"V{version + 1}";

            FileHelper.CreateFolderIfNotExist(Path.Combine(_basePath, versionName));

            var oldContent = FileHelper.GetFolderContent(Path.Combine(_basePath, CurrentVersion.ToString()));

            DataVersions.Clear();
            DataVersions.AddRange(FileHelper.GetVersions(_basePath));

            MaxLoaderValue = oldContent.Count();
            IsFileDataLoading = true;

            Task.Run(() =>
            {
                int counter = 0;
                foreach (var item in oldContent)
                {
                    CurrentLoaderValue = counter;

                    var response = item.Copy(Path.Combine(_basePath, versionName), false);
                    if (!response.Success)
                    {
                        PublishSnackBar(response.Message);
                        IsFileDataLoading = false;
                        return;
                    }
                    counter++;
                }

                CurrentPath = Path.Combine(_basePath, versionName);
                CurrentVersion = DataVersions.Where(v => v.Name.Equals(versionName)).FirstOrDefault();
                if (CurrentVersion == null)
                {
                    PublishSnackBar("Somethinh went wrong..");
                    IsFileDataLoading = false;
                    return;
                }

                PublishSnackBar($"New version added successfully!");
                IsFileDataLoading = false;
            });

        }

        private void MoveFile(IData item, string desstinationPath, FileAction fileAction, bool overwrite, bool addToRelatedFiles = true)
        {
            Response moveResponse;
            if (fileAction == FileAction.CopyAndReplace || fileAction == FileAction.Copy)
            {
                moveResponse = item.Copy(desstinationPath, overwrite);
            }
            else
            {
                moveResponse = item.Move(desstinationPath, overwrite);
            }

            if (!moveResponse.Success)
            {
                PublishSnackBar($"Failed to copy file: {moveResponse.Message}");
                return;
            }

            if (FileHelper.Contains(RelatedFiles, Path.GetFileName(item.Path))) return;

            if(addToRelatedFiles) App.Current.Dispatcher.Invoke(() => RelatedFiles.Add(item));
        }

        public void OnFileDrop(string[] filePaths)
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            if (filePaths == null || filePaths.Length == 0) return;
            var response = FileHelper.GetFilesFromPath(filePaths);
            if (!string.IsNullOrEmpty(response.Message))
            {
                PublishSnackBar($"Failed to move files: {response.Message}");
                return;
            }

            AppSettings.Load();
            var fileAction = AppSettings.Instance.FileAction;

            var files = response.Data;
            var overwrite = (fileAction == FileAction.CopyAndReplace || fileAction == FileAction.MoveAndReplace) ? true : false;

            Task.Run(() =>
            {
                foreach (var item in files)
                {
                    try
                    {
                        MoveFile(item, CurrentPath, fileAction, overwrite);
                    }
                    catch (Exception e)
                    {
                        PublishSnackBar($"Couldn't move file: {e.Message}");
                    }
                }

                PublishSnackBar($"Files have been moved!");

            });
            
        }

        public void OnInnerFileDrop(IData sourceFile, IData destinationFile)
        {
            var fileAction = FileAction.Move;
            var overwrite = false;
            Task.Run(() =>
            {
                try
                {
                    MoveFile(sourceFile, destinationFile.Path, fileAction, overwrite, false);
                    App.Current.Dispatcher.Invoke(() => RelatedFiles.Remove(sourceFile));
                }
               catch(Exception e)
                {
                    PublishSnackBar($"Couldn't move file: {e.Message}");
                    return;
                }

                PublishSnackBar($"File has been moved!");
            });
        }

        #endregion // Files

        #region Contributors / Details

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

        private void AddContributor()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            if (string.IsNullOrEmpty(SelectedContributor))
            {
                PublishSnackBar("Cannot add this person...");
                return;
            }
            var names = SelectedContributor.Trim().Split(' ');
            if (names.Length < 2)
            {
                PublishSnackBar("Please enter full name");
                return;
            }
            var contributor = _repository.GetContributor(names);

            if (contributor == null)
            {
                contributor = new Contributor
                {
                    FirstName = names[0],
                    LastName = names[1],
                    ID = Guid.NewGuid().ToString()
                };

                _repository.Insert(contributor, "contributors");
                AllContributors.Add(contributor);
            }

            if (Contributors.Where(c => c.ID.Equals(contributor.ID)).FirstOrDefault() != null)
            {
                PublishSnackBar("Contributor already added!");
                return;
            }

            if (_currentProject != null && !_repository.AssignContributor(_currentProject.ID, contributor.ID))
            {
                PublishSnackBar("Failed to assign contributor...");
                return;
            }

            PublishSnackBar("Contributor has been added!");
            Contributors.Add(contributor);
            SelectedContributor = string.Empty;
        }

        public void SetSelectedContributors(IEnumerable<Contributor> list)
        {
            SelectedContributors.Clear();
            SelectedContributors.AddRange(list);
            SelectedContributorCount = SelectedContributors.Count;
        }

        public void SetDeselectAction(Action action)
        {
            _deselectContributors = action;
        }

        #endregion // Contributors / Details

        #region Comments / Links

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

        #endregion // Comments Links

        private void ProjectReceived(Project project)
        {
            _currentProject = project;
            if (project == null) return;

            Contributors.AddRange(_currentProject.Contributors);
            ProjectDescription = project.Content;
            SeparateComments();

            _basePath = Path.Combine(".\\Projects", _currentProject.ID);
            CurrentPath = _basePath;
            FileHelper.CreateFolderIfNotExist(_basePath);

            DataVersions.AddRange(FileHelper.GetVersions(_basePath));
            if (DataVersions.Count == 0)
            {
                FileHelper.CreateFolderIfNotExist(Path.Combine(_basePath, "V1"));
                DataVersions.AddRange(FileHelper.GetVersions(_basePath));
            }

            CurrentPath = Path.Combine(_basePath, DataVersions[DataVersions.Count - 1].ToString());
            IsFileDataLoading = true;
          
           
            Task.Run(() =>
            {
                CurrentVersion = DataVersions[DataVersions.Count - 1];
                IsFileDataLoading = false;
            });
           
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

    }
}
