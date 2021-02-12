using Sakuno.ING.Composition;
using System.Windows;

namespace Sakuno.ING.Shell.Desktop
{
    internal partial class ShellApp
    {
        public ShellApp()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Compositor.Default.Resolve<MainWindow>();

            MainWindow = mainWindow;

            mainWindow.Show();
        }
    }
}
