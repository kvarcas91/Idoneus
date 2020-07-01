using Idoneus.ViewModels;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl { 

        readonly IContainerExtension _container;
        readonly IRegionManager _regionManager;
        private IRegion  _taskRegion;

        private DashboardDailyTasks _dashboardDailyTasks;

        public Dashboard(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            _container = container;
            _regionManager = regionManager;
            Loaded += Dashboard_Loaded;
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            _dashboardDailyTasks = _container.Resolve<DashboardDailyTasks>();
            _taskRegion = _regionManager.Regions["DashboardTaskRegion"];
            _taskRegion.Add(_dashboardDailyTasks);
          
            ((DashboardViewModel)DataContext).InitRegion();

        }
    }
}
