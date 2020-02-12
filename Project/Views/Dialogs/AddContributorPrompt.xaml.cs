using Core.DataModels;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;
using Core.DataBase;

namespace Idoneus.Dialogs
{

    public partial class AddContributorPrompt : Window
    {
        public ObservableCollection<IContributor> Contributors { get; set; } = new ObservableCollection<IContributor>();
        private ObservableCollection<IContributor> selectedContributors = new ObservableCollection<IContributor>();
        private object param;
        public static AddContributorPrompt Instance;

        public AddContributorPrompt()
        {
            InitializeComponent();
            Instance = this;
        }

        public AddContributorPrompt(ObservableCollection<IContributor> contributors, object param) : this ()
        {
            Instance.param = param;
            SortContributors(contributors);
            AddContributorList.ItemsSource = Contributors;
        }

        private void SortContributors(ObservableCollection<IContributor> contributors)
        {
            ObservableCollection<IContributor> allContributor;
            if (Instance.param is IProject) allContributor = DBHelper.GetAllContributors();
            else allContributor = DBHelper.GetProjectContributors(DBHelper.GetProjectIDFromTask(((ITask)Instance.param).ID));
            
            foreach (var contributor in allContributor)
            {
                var contr = (Contributor)contributor;
                if (contributors.Contains(contr))
                {
                    contr.IsSelected = true;
                    Contributors.Insert(selectedContributors.Count, contr);
                    selectedContributors.Add(contr);
                }
                else Contributors.Add(contr);
            }
        }

        public static ObservableCollection<IContributor> ShowDialog(object param, ObservableCollection<IContributor> contributors)
        {
            AddContributorPrompt prompt = new AddContributorPrompt(contributors, param);
           
            if (param is ITask) prompt.AddContributorPanel.Visibility = Visibility.Collapsed;

            prompt.ShowDialog();

            return prompt.selectedContributors;
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var contributor = ((CheckBox)sender).Tag;
            if (!selectedContributors.Contains((Contributor)contributor))
            {
                ((Contributor)contributor).IsSelected ^= true;
                if (param is IProject project) DBHelper.AssignContributors(project.ID, ((Contributor)contributor).ID);
                if (param is ITask task) DBHelper.AssignTaskContributors(task.ID, ((Contributor)contributor).ID);
                //DBHelper.UpdateContributor((Contributor)contributor);
                selectedContributors.Add((Contributor)contributor);
            }
            else
            {
                if (param is IProject project) DBHelper.ReAssignContributor(project.ID, ((Contributor)contributor).ID);
                if (param is ITask task) DBHelper.ReAssignTaskContributor(task.ID, ((Contributor)contributor).ID);
                selectedContributors.Remove((Contributor)contributor);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddNewContributor(object sender, RoutedEventArgs e)
        {
            if (SendCommentTextBox.Text.Trim(' ').Split(' ').Length < 2)
            {
                MessageBox.Show("First and last names are required");
                return;
            }
            var contributor = new Contributor(SendCommentTextBox.Text);
            DBHelper.InsertContributor(contributor);

            contributor.IsSelected = true;
            Contributors.Insert(selectedContributors.Count, contributor);
            selectedContributors.Add(contributor);
            if (param is IProject project) DBHelper.AssignContributors(project.ID, contributor.ID);
            SendCommentTextBox.Clear();
        }
    }
}
