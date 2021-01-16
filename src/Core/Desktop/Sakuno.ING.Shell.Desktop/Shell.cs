using ReactiveUI;
using Sakuno.ING.Composition;
using Splat;
using System.Windows;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IShell))]
    internal class Shell : IShell
    {
        public void Run()
        {
            var app = new Application()
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose,
            };
            var mainWindow = Compositor.Default.Resolve<MainWindow>();

            Locator.CurrentMutable.InitializeReactiveUI();

            mainWindow.Show();

            app.MainWindow = mainWindow;
            app.Run();
        }
    }
}
