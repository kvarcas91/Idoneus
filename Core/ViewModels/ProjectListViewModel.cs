using Core.DataModels;
using Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core.ViewModels
{
    public class ProjectListViewModel : BaseViewModel
    {

        #region Observable Collections

        public ObservableCollection<IProject> Projects { get; set; }
        public ObservableCollection<ITask> Tasks { get; set; }
        public ObservableCollection<IComment> Comments { get; set; }

        #endregion // Observable Collections

        #region Icommand Properties

        public ICommand TestCommand { get; set; }
        public ICommand CollapseProjectListCommand { get; set; }

        #endregion // Icommand Properties

        #region Icommand Methods



        #endregion // Icommand Methods

        #region Public Properties

        public IProject CurrentProject { get; set; } = FakeData.GetProject();

        #region Visilibity

        public bool IsProjectListSideBarExpanded { get; set; } = true;

        #endregion // Visibility

        #region Info Box

        public double TotalTasksProgress
        {
            get => CompletedTasksCount * 100 / (CompletedTasksCount + OverdueTasksCount);
            set => TotalTasksProgress = value;
        }
        public int CompletedTasksCount { get; set; } = 120;
        public int OverdueTasksCount { get; set; } = 100;

        #endregion // Info Box

        #endregion // Public Properties

        #region Private Properties



        #endregion // Private Properties

        #region Private Methods

        private void SetUpCommands ()
        {
            CollapseProjectListCommand = new RelayCommand(CollapseProjectList);
        }

        private void CollapseProjectList ()
        {
            IsProjectListSideBarExpanded ^= true;
        }

        #endregion // Private Methods

        #region Constructor

        public ProjectListViewModel()
        {
            Projects = FakeData.GetProjects();
            Tasks = FakeData.GetTasks();

            SetUpCommands();
            InitTest();
        }

        #endregion // Constructor


        #region Test

        private void InitTest ()
        {
            TestCommand = new ParameterizedRelayCommand<IProject>(Test);
        }

        private void Test (IProject project)
        {
            CurrentProject = project;
        }

        #endregion // Test
    }
}
