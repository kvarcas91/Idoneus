using Core.DataBase;
using Core.DataModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Idoneus.Dialogs
{
    /// <summary>
    /// Interaction logic for MoveTaskPrompt.xaml
    /// </summary>
    public partial class MoveTaskPrompt : Window
    {
        private bool isChanged = false;
        private static MoveTaskPrompt instance;
        private readonly IElement element;
        public ObservableCollection<IProject> Projects { get; set; } = new ObservableCollection<IProject>();
        public ObservableCollection<IElement> Tasks { get; set; } = new ObservableCollection<IElement>();

        public MoveTaskPrompt()
        {
            InitializeComponent();
            instance = this;
        }

        public MoveTaskPrompt(IElement element) : this()
        {
            this.element = element;
            var projects = DBHelper.GetProjects(Core.Utils.ViewType.Ongoing);
            Tasks = DBHelper.GetAllActiveTasks();
            foreach (var item in projects)
            {
                Projects.Add(item);
            }
            ProjectList.ItemsSource = Projects;
            TaskList.ItemsSource = Tasks;
        }

        public static bool ShowDialog (IElement element)
        {
            MoveTaskPrompt prompt = new MoveTaskPrompt(element);
            prompt.ShowDialog();
            return instance.isChanged;
        }

        private void OnProjectListItemClick(object sender, RoutedEventArgs e)
        {
            var destinationProject = ((TreeViewItem)e.OriginalSource).DataContext as IProject;
            
            if (element is ITask task)
            {
                if (destinationProject.Tasks.Contains(element)) return;
                DBHelper.ReAssignTaskFromTheProject(destinationProject.ID, task.ID);
                isChanged = true;
            }
            if (element is ISubTask subTask)
            {
                MoveSubtaskToProject(subTask, destinationProject);
                isChanged = true;
            }

            Discard();
        }

        private void MoveSubtaskToProject (ISubTask subTask, IElement destination)
        {
            var task = new Core.DataModels.Task
            {
                Content = subTask.Content,
                SubmitionDate = subTask.SubmitionDate,
                DueDate = subTask.DueDate,
                Priority = subTask.Priority,
                IsCompleted = subTask.IsCompleted
            };

            //var oldSubtaskProject = DBHelper.GetProjectFromSubtask(subTask.ID);

            DBHelper.InsertTask(task, destination.ID);
            DBHelper.ReAssignSubTaskFromTask(subTask.ID);

            
            //DBHelper.DeleteSubTask(subTask);
        }

        private void Discard(object sender = null, RoutedEventArgs e = null)
        {
            Close();
        }
    }
}
