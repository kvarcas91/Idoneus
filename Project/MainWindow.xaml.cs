using Core.Utils;
using Idoneus.ViewModel;
using Squirrel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }

        private void CheckForUpdates ()
        {
            Task.Run(async () =>
            {


                try
                {
                    using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/kvarcas91/Idoneus"))
                    {
                        await mgr.Result.UpdateApp();
                       
                        var status = mgr.Status;
                        var result = mgr.Result;
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message + Environment.NewLine;
                    if (ex.InnerException != null)
                        message += ex.InnerException.Message;
                    Console.WriteLine(message);
                }
                //var updateInfo = mgr.CheckForUpdate();
                // var updateInfo = await mgr.CheckForUpdate();
                //if (updateInfo.ReleasesToApply.Any())
                //   ReleaseEntry release = await mgr.UpdateApp();
                // if (release != null)
                //UpdateManager.RestartApp();
            });

        }

    }
}
