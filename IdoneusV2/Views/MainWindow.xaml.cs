using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace IdoneusV2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IContainerExtension _container;
        readonly IRegionManager _regionManager;
        private IRegion _region;

        SideBar _sidebarView;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            _container = container;
            _regionManager = regionManager;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _sidebarView = _container.Resolve<SideBar>();
            _region = _regionManager.Regions["SideBarRegion"];
            _region.Add(_sidebarView);
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {

        }

        private void Maximize(object sender, RoutedEventArgs e)
        {

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
