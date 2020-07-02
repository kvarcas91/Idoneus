using Domain.Models;
using Idoneus.ViewModels;
using System.Linq;
using System.Windows.Controls;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for Tasks.xaml
    /// </summary>
    public partial class Tasks : UserControl
    {

        private readonly TasksViewModel viewModel;

        public Tasks()
        {
            InitializeComponent();
            viewModel = (TasksViewModel)DataContext;
            viewModel.SetDeselectAction(DeselectContributors);
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SetSelectedContributors(contributorList.SelectedItems.Cast<Contributor>());
        }

        private void DeselectContributors()
        {
            contributorList.UnselectAll();
        }
    }
}
