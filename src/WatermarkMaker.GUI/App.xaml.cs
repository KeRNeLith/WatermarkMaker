using System.Windows;
using Prism.Ioc;
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
            return Container.Resolve<MainWindow>();
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
        }
    }
}