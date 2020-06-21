using Idoneus.Helpers;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Idoneus.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly IContainerExtension _container;
        readonly IRegionManager _regionManager;
        IRegion _region;
        Dashboard _dashboard;
        Projects _projects;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowSizeHelper.WindowProc));
            };

            _container = container;
            _regionManager = regionManager;

            Loaded += MainWindow_Loaded;
            CloseButton.Click += (s, e) => this.Close();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dashboard = _container.Resolve<Dashboard>();
            _projects = _container.Resolve<Projects>();
            _region = _regionManager.Regions["ContentRegion"];
            _region.Add(_dashboard);
            _region.Add(_projects);
        }
    }
}
