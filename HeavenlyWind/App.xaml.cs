using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.Views;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            DispatcherUtil.UIDispatcher = Dispatcher;

            Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);

            if (!Debugger.IsAttached)
            {
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }

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

            ThemeManager.Instance.Initialize(this, Accent.Blue);

            CoreDatabase.Initialize();

            DataService.Instance.EnsureDirectory();

            RecordService.Instance.Initialize();
            QuestProgressService.Instance.Initialize();
            MapService.Instance.Initialize();
            ExpeditionService.Instance.Initialize();
            EnemyEncounterService.Instance.Initialize();

            Preference.Instance.Initialize();
            Preference.Instance.Reload();
            StringResources.Instance.Initialize();
            StringResources.Instance.LoadMainResource(Preference.Instance.Language);
            StringResources.Instance.LoadExtraResource(Preference.Instance.ExtraResourceLanguage);

            StatusBarService.Instance.Initialize();
            CacheService.Instance.Initialize();
            NotificationService.Instance.Initialize();

            ServiceManager.Register<IBrowserService>(BrowserService.Instance);

            PluginService.Instance.Initialize();

            KanColleProxy.Start();

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            Task.Factory.StartNew(UpdateService.Instance.CheckForUpdate);

            if (e.Args.Any(r => r.OICEquals("--background")))
                return;

            MainWindow = new MainWindow();
            MainWindow.DataContext = Root = new MainWindowViewModel();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            NotificationService.Instance.Dispose();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            ShowUnhandledExceptionDialog(e.Exception);
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnhandledExceptionDialog((Exception)e.ExceptionObject);
        }

        void ShowUnhandledExceptionDialog(Exception rpException)
        {
            var rLogFilename = Logger.GetNewExceptionLogFilename();
            try
            {
                using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                {
                    rStreamWriter.WriteLine("Unhandled Exception:");
                    rStreamWriter.WriteLine();
                    rStreamWriter.WriteLine(rpException.ToString());
                    rStreamWriter.WriteLine();
                }
            }
            catch
            {
                rLogFilename = null;
            }

            var rDialog = new TaskDialog()
            {
                Caption = UnhandledExceptionDialogStringResources.ProductName,
                Instruction = UnhandledExceptionDialogStringResources.Instruction,
                Icon = TaskDialogIcon.Error,
                Content = UnhandledExceptionDialogStringResources.Content,

                Detail = rpException.ToString(),
                ShowDetailAtTheBottom = true,

                OwnerWindow = MainWindow,
                ShowAtTheCenterOfOwner = true,
            };

            if (rLogFilename != null)
            {
                rDialog.EnableHyperlinks = true;
                rDialog.FooterIcon = TaskDialogIcon.Information;
                rDialog.Footer = string.Format(UnhandledExceptionDialogStringResources.Footer, $"<a href=\"{rLogFilename}\">{rLogFilename}</a>");

                rDialog.HyperlinkClicked += delegate
                {
                    if (File.Exists(rLogFilename))
                        Process.Start(rLogFilename);
                }; ;
            }

            rDialog.ShowAndDispose();
        }
    }
}
