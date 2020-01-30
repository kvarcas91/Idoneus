using Core.DataModels;
using Core.Helpers;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Idoneus.Dialogs
{
    /// <summary>
    /// Interaction logic for AddProjectPrompt.xaml
    /// </summary>
    public partial class AddProjectPrompt : Window
    {

        private Core.DataModels.Project project = new Core.DataModels.Project();

        public AddProjectPrompt()
        {
            InitializeComponent();
            dueTime.SelectedDate = DateTime.Now;
            
        }

        public static Core.DataModels.Project Show()
        {
            AddProjectPrompt prompt = new AddProjectPrompt();

            prompt.ShowDialog();
            return prompt.project;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            project = null;
            Close();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            if (!StringHelper.CanUse(Title.Text) || !StringHelper.CanUse(Description.Text)) return;

            project.Header = Title.Text;
            project.Content = Description.Text;
            project.SubmitionDate = DateTime.Now;
            project.DueDate = (DateTime)dueTime.SelectedDate;
            project.Priority = (Priority)dueCombo.SelectedItem;
            project.Path = "test/path";
            Close();
        }
    }
}
