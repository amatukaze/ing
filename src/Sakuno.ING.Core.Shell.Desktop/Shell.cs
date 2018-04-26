using Sakuno.ING.Settings;
using Sakuno.ING.ViewModels;
using System;
using System.Windows;

namespace Sakuno.ING.Shell
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

        public void RegisterSettingView(Type viewType, SettingCategory category = SettingCategory.Misc) => throw new NotImplementedException();
        public void RegisterView(Type viewType, string id, bool isFixedSize = true, bool singleWindowRecommended = false) => throw new NotImplementedException();

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
