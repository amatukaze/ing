using Sakuno.KanColle.Amatsukaze.Game.Proxy;
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
            base.OnStartup(e);

            if (e.Args.Length >= 3 && e.Args[0] == "browser")
            {

                var rLayoutEngine = e.Args[1];
                var rHostProcessID = int.Parse(e.Args[2]);

                new BrowserWrapper(rLayoutEngine, rHostProcessID);

                Task.Factory.StartNew(() =>
                {
                    Process.GetProcessById(rHostProcessID).WaitForExit();
                    Environment.Exit(0);
                }, TaskCreationOptions.LongRunning);

                return;
            }

            StatusBarService.Instance.Initialize();

            Preference.Load();
            StringResources.Instance.Load(Preference.Current.Language);

            KanColleProxy.Start();

            MainWindow = new MainWindow();
            MainWindow.DataContext = Root = new MainWindowViewModel();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Preference.Save();
        }
    }
}
