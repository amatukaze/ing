using Sakuno.ING.Composition;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels;
using Sakuno.UserInterface;
using System.Windows;

namespace Sakuno.ING.Shell
{
    [Export(typeof(IShell))]
    class AppShell : IShell
    {
        public void Run()
        {
            var app = new ThemedApp() { ShutdownMode = ShutdownMode.OnMainWindowClose };

            app.Startup += OnAppStartup;

            app.Run();
        }

        void OnAppStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow()
            {
                DataContext = Compositor.Static<MainViewModel>(),
            };

            mainWindow.Show();

            Application.Current.MainWindow = mainWindow;
        }
    }
}
