using Core.Utils;
using Idoneus.ViewModel;
using Squirrel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Idoneus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            CheckForUpdates();
            AddVersionNumber();
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }

        private void CheckForUpdates ()
        {
            Task.Run(async () =>
            {
                using (var mgr = new UpdateManager(@"https://github.com/kvarcas91/Idoneus"))
                {
                    var updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo == null || updateInfo.ReleasesToApply.Count == 0)
                    {

                        return;

                    }

                    await mgr.DownloadReleases(updateInfo.ReleasesToApply);
                    await mgr.ApplyReleases(updateInfo);


                }
            });

        }

        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine(versionInfo);
        }
    }
}
