using Domain.Models;
using Idoneus.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {
        public Details()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = (DetailsViewModel)DataContext;
            viewmodel.SelectedContributors.Clear();
            viewmodel.SelectedContributors.AddRange(contributorList.SelectedItems.Cast<Contributor>());
            viewmodel.Test();
        }
    }
}
