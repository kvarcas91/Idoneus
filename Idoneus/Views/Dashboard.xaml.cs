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
        private IRegion _projectRegion, _taskRegion;

        private DashboardDailyTasks _dashboardDailyTasks;
        private DashboardProjects _dashboardProjects;
        private DashboardUpcommingTasks _dashboardUpcommingTasks;

        public Dashboard(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();

            _container = container;
            _regionManager = regionManager;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dashboardDailyTasks = _container.Resolve<DashboardDailyTasks>();
            _dashboardProjects = _container.Resolve<DashboardProjects>();
            _dashboardUpcommingTasks = _container.Resolve<DashboardUpcommingTasks>();

            _projectRegion = _regionManager.Regions["DashboardProjectRegion"];
            _projectRegion.Add(_dashboardProjects);
            _projectRegion.Add(_dashboardUpcommingTasks);

            _taskRegion = _regionManager.Regions["DashboardTaskRegion"];
            _taskRegion.Add(_dashboardDailyTasks);

            _projectRegion.Activate(_dashboardProjects);
            ((DashboardViewModel)DataContext).ActivateTab();

        }
    }
}
