using Sakuno.KanColle.Amatsukaze.ViewModels;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    class Shell : IShell
    {
        MainWindowVM _mainWindowVM;

        public Shell()
        {
            _mainWindowVM = new MainWindowVM()
            {
                Title = "Intelligent Naval Gun",
            };
        }

        public void Run()
        {
            var app = new App()
            {
                MainWindow = new MainWindow() { DataContext = _mainWindowVM },
            };

            app.MainWindow.Show();

            app.Run();
        }
    }
}
