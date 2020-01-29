using Core.DataModels;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;
using Core.DataBase;

namespace Project.Dialogs
{
    /// <summary>
    /// Interaction logic for PromptTest.xaml
    /// </summary>
    public partial class AddContributorPrompt : Window
    {
        public ObservableCollection<IContributor> Contributors { get; set; } = new ObservableCollection<IContributor>();
        private ObservableCollection<IContributor> selectedContributors = new ObservableCollection<IContributor>();
        private static AddContributorPrompt instance;
        private long projectID;

        public AddContributorPrompt()
        {
            InitializeComponent();
            instance = this;
            
        }

        public AddContributorPrompt(ObservableCollection<IContributor> contributors) : this ()
        {
            
            SortContributors(contributors);
            AddContributorList.ItemsSource = Contributors;
        }

        private void SortContributors(ObservableCollection<IContributor> contributors)
        {
            ObservableCollection<IContributor> allContributor = DBHelper.GetAllContributors();
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

        public static ObservableCollection<IContributor> ShowDialog(long projectID, ObservableCollection<IContributor> contributors)
        {
            //Window prompt = new Window();
            
           
            Window prompt = new AddContributorPrompt(contributors);
            instance.projectID = projectID;
            
            //prompt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //prompt.Width = 500;
            //prompt.Height = 100;
            
            //StackPanel panel = new StackPanel() { Orientation = Orientation.Vertical };

            //Button confirmation = new Button() { Content = "Ok" };
            //confirmation.Click += (sender, e) => { prompt.Close(); };

            //panel.Children.Add(confirmation);
            //prompt.Content = panel;
            //prompt.ShowDialog();
            foreach (var contributor in contributors)
            {
                Console.WriteLine(contributor.ToString());
            }
            prompt.ShowDialog();
            return instance.selectedContributors;
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var contributor = ((CheckBox)sender).Tag;
            if (!selectedContributors.Contains((Contributor)contributor))
            {
                ((Contributor)contributor).IsSelected ^= true;
                DBHelper.AssignContributors(projectID, ((Contributor)contributor).ID);
                //DBHelper.UpdateContributor((Contributor)contributor);
                selectedContributors.Add((Contributor)contributor);
            }
            else
            {
                DBHelper.ReAssignContributor(projectID, ((Contributor)contributor).ID);
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
            DBHelper.AssignContributors(projectID, contributor.ID);
            SendCommentTextBox.Clear();
        }
    }
}
