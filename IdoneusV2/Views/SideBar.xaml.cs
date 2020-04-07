using Prism.Ioc;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IdoneusV2.Views
{
    /// <summary>
    /// Interaction logic for SideBar.xaml
    /// </summary>
    public partial class SideBar : UserControl
    {

        IContainerExtension _container;
        IRegionManager _regionManager;
        IRegion _region;
        DashboardView _dashboardContent;

        public SideBar(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            _container = container;
            _regionManager = regionManager;

            Loaded += SideBar_Loaded;
        }

        private void SideBar_Loaded(object sender, RoutedEventArgs e)
        {
            _dashboardContent = _container.Resolve<DashboardView>();
           _region = _regionManager.Regions["ContentRegion"];
            _region.Add(_dashboardContent);
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _region.Activate(_dashboardContent);
        }
    }
}
