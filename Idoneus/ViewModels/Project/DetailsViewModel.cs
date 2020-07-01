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
using System.Diagnostics;
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

        private readonly IEventAggregator _eventAggregator;
        private readonly ProjectRepository _repository;
        private Action _deselectContributors;

        #endregion // Local members

        #region Properties

        #region Contributors

        private int _selectedContributorCount = 0;
        public int SelectedContributorCount
        {
            get { return _selectedContributorCount; }
            set { SetProperty(ref _selectedContributorCount, value); }
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

        private bool _isFileEditable = false;
        private IData _editableData;

        #endregion // Files

        #region Comments / Links

        private bool _isCommentExpanded = false;
        public bool IsCommentExpanded
        {
            get { return _isCommentExpanded; }
            set { SetProperty(ref _isCommentExpanded, value); }
        }

        private int _commentsViewType = 0;
        public int CommentsViewType
        {
            get { return _commentsViewType; }
            set { SetProperty(ref _commentsViewType, value); }
        }

        private string _commentText;
        public string CommentText
        {
            get { return _commentText; }
            set { SetProperty(ref _commentText, value); }
        }

        private string _linkText;
        public string LinkText
        {
            get { return _linkText; }
            set { SetProperty(ref _linkText, value); }
        }

        private string _linkHeaderText;
        public string LinkHeaderText
        {
            get { return _linkHeaderText; }
            set { SetProperty(ref _linkHeaderText, value); }
        }

        private Comment _previewComment;
        public Comment PreviewComment
        {
            get { return _previewComment; }
            set { SetProperty(ref _previewComment, value); }
        }

        #endregion // Comments / Links

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

        #region Contributors / Details

        private DelegateCommand _deleteSelectedContributorsCommand;
        public DelegateCommand DeleteSelectedContributorsCommand => _deleteSelectedContributorsCommand ?? (_deleteSelectedContributorsCommand = new DelegateCommand(DeleteSelectedContributors));

        private DelegateCommand _unselectContributorsCommand;
        public DelegateCommand UnselectContributorsCommand => _unselectContributorsCommand ?? (_unselectContributorsCommand = new DelegateCommand(UnselectContributors));

        private DelegateCommand _addContributorCommand;
        public DelegateCommand AddContributorCommand => _addContributorCommand ?? (_addContributorCommand = new DelegateCommand(AddContributor));

        #endregion // Contributors / Details

        #region Files

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

        private DelegateCommand<IData> _editFileCommand;
        public DelegateCommand<IData> EditFileCommand => _editFileCommand ?? (_editFileCommand = new DelegateCommand<IData>(EditFile));

        #endregion // Files

        #region Comments / Links

        private DelegateCommand _addCommentCommand;
        public DelegateCommand AddCommentCommand => _addCommentCommand ?? (_addCommentCommand = new DelegateCommand(AddComment));

        private DelegateCommand _updateCommentCommand;
        public DelegateCommand UpdateCommentCommand => _updateCommentCommand ?? (_updateCommentCommand = new DelegateCommand(UpdateComment));

        private DelegateCommand<Comment> _expandCommentCommand;
        public DelegateCommand<Comment> ExpandCommentCommand => _expandCommentCommand ?? (_expandCommentCommand = new DelegateCommand<Comment>(ExpandComment));

        private DelegateCommand _addLinkCommand;
        public DelegateCommand AddLinkCommand => _addLinkCommand ?? (_addLinkCommand = new DelegateCommand(AddLink));

        private DelegateCommand<IComment> _openLinkCommand;
        public DelegateCommand<IComment> OpenLinkCommand => _openLinkCommand ?? (_openLinkCommand = new DelegateCommand<IComment>(OpenLink));

        private DelegateCommand<IComment> _deleteCommentCommand;
        public DelegateCommand<IComment> DeleteCommentCommand => _deleteCommentCommand ?? (_deleteCommentCommand = new DelegateCommand<IComment>(DeleteComment));

        #endregion // Comments / Links

        #endregion // Delegates

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

        #region Methods

        #region Files

        private void EditFile(IData data)
        {
            ShowAddFolderPanel();
            _editableData = data;
            _isFileEditable = true;
            FolderName = data.Name;
        }

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
            catch (Exception e)
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
                PublishSnackBar("Name cannot be emty");
                return;
            }

            if (_isFileEditable)
            {
                Edit();
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

        private void Edit()
        {
            if (FolderName.Equals(_editableData.Name))
            {
                FolderName = string.Empty;
                IsAddFolderPanelVisible = false;
                _editableData = null;
                return;
            }

            var response = _editableData.Rename(FolderName);
            if (!response.Success)
            {
                PublishSnackBar($"Failed to rename: {response.Message}");
                return;
            }

            var index = RelatedFiles.IndexOf(_editableData);
            RelatedFiles.Remove(_editableData);
            if (index > -1) RelatedFiles.Insert(index, _editableData);
            else RelatedFiles.Add(GetNew(_editableData));

            FolderName = string.Empty;
            IsAddFolderPanelVisible = false;
            _editableData = null;
        }

        private IData GetNew(IData data)
        {
            if (data is ProjectFolder) return new ProjectFolder(data.Path);
            else return new ProjectFile(data.Path);
        }

        private void ShowAddFolderPanel()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }

            FolderName = string.Empty;
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

                    var response = item.Copy(Path.Combine(_basePath, versionName), false, true);
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

            if (addToRelatedFiles) App.Current.Dispatcher.Invoke(() => RelatedFiles.Add(item));
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
                catch (Exception e)
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

        private void UpdateComment()
        {
            if (PreviewComment == null)
            {
                PublishSnackBar("Cannot update comment...");
                return;
            }

            if (string.IsNullOrEmpty(PreviewComment.Content))
            {
                PublishSnackBar("Content cannot be empty...");
                return;
            }

            if(!_repository.Update(PreviewComment))
            {
                PublishSnackBar("Failed to update comment...");
                return;
            }

            ExpandComment(null);
        }

        private void ExpandComment(Comment comment)
        {
            PreviewComment = comment;
            IsCommentExpanded = !IsCommentExpanded;
        }

        private void DeleteComment(IComment data)
        {
            Task.Run(() =>
            {
                var response = _repository.DeleteComment(PreviewComment ?? data);
                PublishSnackBar(response.Success ? "Deleted successfully!" : "Failed to delete...");
                if (!response.Success)
                {
                    return;
                }

                IsCommentExpanded = false;

                if ((PreviewComment ?? data) is Comment comment)
                {
                    App.Current.Dispatcher.Invoke(() => Comments.Remove(comment));
                }
                if (data is Link link)
                {
                    App.Current.Dispatcher.Invoke(() => Links.Remove(link));
                }

             
            });
        }

        private void OpenLink(IComment link)
        {
            try
            {
                ProcessHelper.RunLink(link.Content);
            }
            catch (Exception e)
            {
                PublishSnackBar($"Cannot open url: {e.Message}");
            }
          
        }

        private void AddLink()
        {
            if (_currentProject == null) return;
            if (string.IsNullOrEmpty(LinkText))
            {
                PublishSnackBar("Please add the link");
                return;
            }
            var link = new Link
            {
                ID = Guid.NewGuid().ToString(),
                Content = LinkText,
                ProjectID = _currentProject.ID,
                SubmitionDate = DateTime.Now,
                Header = string.IsNullOrEmpty(LinkHeaderText) ? LinkText : LinkHeaderText
            };

            Task.Run(() =>
            {
                if (_repository.Insert(link, "links"))
                {
                    PublishSnackBar("Link has been added!");
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Links.Insert(0, link);
                    });

                    LinkText = string.Empty;
                    LinkHeaderText = string.Empty;
                }
                else PublishSnackBar("Something went wrong..");
            });
        }
 
        private void AddComment()
        {
            if (_currentProject == null)
            {
                PublishSnackBar("Select project first");
                return;
            }
            if (string.IsNullOrEmpty(CommentText))
            {
                PublishSnackBar("Please add the comment");
                return;
            }

            var comment = new Comment
            {
                ID = Guid.NewGuid().ToString(),
                Content = CommentText,
                ProjectID = _currentProject.ID,
                SubmitionDate = DateTime.Now
            };

            Task.Run(() =>
            {
                if (_repository.Insert(comment, "comments"))
                {
                    PublishSnackBar("Comment has been added!");
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Comments.Insert(0, comment);
                    });

                    CommentText = string.Empty;
                }
                else PublishSnackBar("Something went wrong..");
            });
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

        #endregion // Comments Links

        #region Common

        private void ProjectReceived(Project project)
        {
            _currentProject = project;
            if (project == null) return;

            //Task.Run(() =>
            //{
               // _currentProject = _repository.GetProject(project.ID);

                Task.Run(() =>
                {
                    _basePath = Path.Combine(".\\Projects", _currentProject.ID);
                   // CurrentPath = _basePath;
                    FileHelper.CreateFolderIfNotExist(_basePath);
                   
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        DataVersions.Clear();
                        DataVersions.AddRange(FileHelper.GetVersions(_basePath));
                        if (DataVersions.Count == 0)
                        {
                            FileHelper.CreateFolderIfNotExist(Path.Combine(_basePath, "V1"));
                            DataVersions.AddRange(FileHelper.GetVersions(_basePath));
                        }
                        CurrentVersion = DataVersions[DataVersions.Count - 1];
                        CurrentPath = Path.Combine(_basePath, DataVersions[DataVersions.Count - 1].ToString());
                    });
                    

                });

                if (Contributors != null) App.Current.Dispatcher.Invoke(() =>
                {
                    Contributors.Clear();
                    Contributors.AddRange(_currentProject.Contributors);
                    SeparateComments();
                });

              

             
              
           // });
          
        }

        private void PublishSnackBar(string text)
        {
            _eventAggregator.GetEvent<SendSnackBarMessage>().Publish(text);
        }

        #endregion // Common

        #endregion // Methods

    }
}
