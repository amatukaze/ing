using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    class Shell : IShell
    {
        public void Run()
        {
            var app = new Application()
            {
                MainWindow = new Window(),
                ShutdownMode = ShutdownMode.OnMainWindowClose,
            };

            app.MainWindow.Show();

            app.Run();
        }
    }
}
