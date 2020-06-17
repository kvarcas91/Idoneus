using Common;
using Domain.Repository;
using Idoneus.Commands;
using Idoneus.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace Idoneus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterSingleton<IStorage, Storage>();
            // register other needed services here
        }
    }
}
