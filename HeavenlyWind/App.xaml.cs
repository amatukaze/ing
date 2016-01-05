using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static MainWindowViewModel Root { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);

            if (e.Args.Length >= 3 && e.Args[0] == "browser")
            {
                var rLayoutEngine = e.Args[1];
                var rHostProcessID = int.Parse(e.Args[2]);

                new BrowserWrapper(rLayoutEngine, rHostProcessID);

                Task.Factory.StartNew(() =>
                {
                    Process.GetProcessById(rHostProcessID).WaitForExit();
                    Process.GetCurrentProcess().Kill();
                }, TaskCreationOptions.LongRunning);

                return;
            }

            StatusBarService.Instance.Initialize();
            RecordService.Instance.Initialize();
            QuestProgressService.Instance.Initialize();

            Preference.Load();
            StringResources.Instance.Load();

            KanColleProxy.Start();

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            Task.Factory.StartNew(UpdateService.Instance.CheckForUpdate);

            MainWindow = new MainWindow();
            MainWindow.DataContext = Root = new MainWindowViewModel();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Preference.Save();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "HeavenlyWind", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
