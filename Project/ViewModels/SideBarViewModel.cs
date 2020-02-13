using Core.DataBase;
using Idoneus.Dialogs;
using Idoneus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Idoneus.ViewModels
{
    public class SideBarViewModel : BaseViewModel
    {

        public string ProfilePicture { get; set; }

        public ICommand DashboardCommand { get; set; }

        public ICommand ProjectsCommand { get; set; }

        public ICommand TasksCommand { get; set; }

        public ICommand AddProjectCommand { get; set; }

        private void OpenDashboard ()
        {
            Debug.WriteLine("OpenDashboard");
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Dashboard);
        }

        private void OpenTasks()
        {
            Debug.WriteLine("OpenTasks");
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Tasks);
        }

        private void OpenProjects()
        {
            Debug.WriteLine("OpenProjects");
            IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Projects);
        }

        public SideBarViewModel()
        {
            DashboardCommand = new RelayCommand(OpenDashboard);
            ProjectsCommand = new RelayCommand(OpenProjects);
            TasksCommand = new RelayCommand(OpenTasks);
            AddProjectCommand = new RelayCommand(AddProject);

            GetProfilePicture();
        }

        public void AddProject()
        {
            var project = AddProjectPrompt.Show();
            if (project != null)
            {
                DBHelper.InsertProject(project);
                IoC.Get<ApplicationViewModel>().GoTo(ApplicationPage.Projects, project);
            }
        }

        private void GetProfilePicture ()
        {
            
            //var local = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //var sep = Path.DirectorySeparatorChar;
            ////var path = $"{local}{sep}Microsoft{sep}Windows{sep}Account Pictures{sep}";
            //var path = "C:\\Users\\eslut\\AppData\\Roaming\\Microsoft\\Windows\\AccountPictures\\";

            //string[] files = Directory.GetFiles(path, "*.accountpicture-ms");
            //if (files.Length > 0) ProfilePicture = files[0];
        }

    }
}
