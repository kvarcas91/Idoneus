using Core.DataBase;
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
        
        private static ProjectListViewModel _instance;
        public static ProjectListViewModel Instance
        {
            get
            {
                if (_instance == null) _instance = new ProjectListViewModel();
                return _instance;
            }
            set => _instance = value;
        }

        #region Observable Collections

        public ObservableCollection<IProject> Projects { get; set; }
        //public IList<IElement> Tasks { get; set; }
        public ObservableCollection<IComment> Comments { get; set; }

        #endregion // Observable Collections

        #region Icommand Properties

        public ICommand TestCommand { get; set; }
        public ICommand CollapseProjectListCommand { get; set; }
        public ICommand ExpandTaskPanelCommand { get; set; }

        #endregion // Icommand Properties

        #region Icommand Methods



        #endregion // Icommand Methods

        #region Public Properties

        public IProject CurrentProject { get; set; }

        #region Counters

       

        #endregion // Counters


        #region Visilibity

        public bool IsProjectListSideBarExpanded { get; set; } = true;
        public bool IsControlExpanded { get; set; } = false;

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
            ExpandTaskPanelCommand = new RelayCommand(ExpandTaskPanel);
        }

       

        private void CollapseProjectList ()
        {
            IsProjectListSideBarExpanded ^= true;
        }

        private void ExpandTaskPanel()
        {
            IsControlExpanded ^= true;
        }

        #endregion // Private Methods

        #region Constructor

        public ProjectListViewModel()
        {

            Instance = this;

            //Projects = FakeData.GetProjects();
            Projects = new ObservableCollection<IProject>(DBHelper.GetProjects(Utils.ViewType.All));
            //Tasks = FakeData.GetTasks();
           
            CurrentProject = (IProject)IoC.Get<ApplicationViewModel>().Parameters;

            //CurrentProject?.Tasks =

           //CurrentProject = FakeData.GetProject();

           IsProjectListSideBarExpanded = (CurrentProject == null);

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
