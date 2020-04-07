using IdoneusV2.Views.Dashboard;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace IdoneusV2.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {

        readonly IContainerExtension _container;
        readonly IRegionManager _regionManager;
        private IRegion _region;
        UpcommingTasks _upcommingTasks;

        public DashboardView(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            _container = container;
            _regionManager = regionManager;

            Loaded += DashboardLoaded;
        }

        private void DashboardLoaded(object sender, RoutedEventArgs e)
        {
            _upcommingTasks = _container.Resolve<UpcommingTasks>();
            _region = _regionManager.Regions["DashboardRegion"];
            _region.Add(_upcommingTasks);
            _region.Activate(_upcommingTasks);
        }
    }
}
