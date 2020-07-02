using Domain.Models;
using Idoneus.ViewModels;
using System.Linq;
using System.Windows.Controls;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {

        private readonly DetailsViewModel viewModel;

        public Details()
        {
            InitializeComponent();
            viewModel = (DetailsViewModel)DataContext;
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
