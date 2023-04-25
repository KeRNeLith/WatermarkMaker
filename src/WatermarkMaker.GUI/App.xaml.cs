using System.Windows;
using Prism.Ioc;
using WatermarkMaker.Threading;
using WatermarkMaker.Views;
using DialogService = MvvmDialogs.DialogService;
using IDialogService = MvvmDialogs.IDialogService;

namespace WatermarkMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal partial class App
    {
        /// <inheritdoc />
        protected override Window CreateShell()
        {
            // This has to be called on the UI thread to have a dispatcher available by injection
            Container.Resolve<IDispatcherService>().Initialize();

            return Container.Resolve<MainWindow>();
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterManySingleton(typeof(DispatcherService), typeof(IDispatcherService), typeof(IDispatcher));
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs args)
        {
            Container.Resolve<IDispatcherService>().Reset();

            base.OnExit(args);
        }
    }
}