using Fiddler;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
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
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Sakuno.KanColle.Amatsukaze
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    partial class App
    {
        public static MainWindowViewModel Root { get; private set; }

        IntPtr r_MainWindowHandle;

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUtil.UIDispatcher = Dispatcher;

            Environment.CurrentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);

            if (!Debugger.IsAttached)
            {
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }

            base.OnStartup(e);

            if (e.Args.Length >= 3 && e.Args[0] == "browser")
            {
                var rLayoutEngine = e.Args[1];
                var rHostProcessID = int.Parse(e.Args[2]);

                BrowserCore(rLayoutEngine, rHostProcessID);
                return;
            }

            ThemeManager.Instance.Initialize(this, Accent.Blue);

            CoreDatabase.Initialize();
            DataStore.Initialize();

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

            EnsureSslCert();

            StatusBarService.Instance.Initialize();
            CacheService.Instance.Initialize();
            NotificationService.Instance.Initialize();

            ServiceManager.Register<IBrowserService>(BrowserService.Instance);

            PluginService.Instance.Initialize();
            Preference.Instance.Reload();

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (!Environment.GetCommandLineArgs().Any(r => r.OICEquals("--no-check-for-update")))
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromHours(4.0)).Subscribe(_ => UpdateService.Instance.CheckForUpdate());

            if (e.Args.Any(r => r.OICEquals("--background")))
                return;

            var rMainWindow = new MainWindow();
            r_MainWindowHandle = rMainWindow.Handle;

            MainWindow = rMainWindow;
            MainWindow.DataContext = Root = new MainWindowViewModel();
            MainWindow.Show();
        }
        async void BrowserCore(string layoutEngine, int hostProcessId)
        {
            var browser = new BrowserWrapper(layoutEngine, hostProcessId);

            browser.Connect();

            await Process.GetProcessById(hostProcessId);

            Process.GetCurrentProcess().Kill();
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
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();

            ShowUnhandledExceptionDialog(e.Exception);
        }

        void ShowUnhandledExceptionDialog(Exception rpException) => Task.Run(() => ShowUnhandledExceptionDialogCore(rpException)).Wait();
        void ShowUnhandledExceptionDialogCore(Exception rpException)
        {
            var rLogFilename = Logger.GetNewExceptionLogFilename();
            try
            {
                using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                {
                    rStreamWriter.WriteLine("Version:");
                    rStreamWriter.WriteLine(ProductInfo.Version);
                    rStreamWriter.WriteLine();
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

                OwnerWindowHandle = r_MainWindowHandle,
                ShowAtTheCenterOfOwner = true,
            };

            if (rLogFilename != null)
            {
                rDialog.EnableHyperlinks = true;
                rDialog.FooterIcon = TaskDialogIcon.Information;
                rDialog.Footer = string.Format(UnhandledExceptionDialogStringResources.Footer, $"<a href=\"{rLogFilename}\">{rLogFilename}</a>");

                EventHandler<string> rHyperlinkClicked = null;
                rHyperlinkClicked = delegate
                {
                    if (File.Exists(rLogFilename))
                        Process.Start(rLogFilename);
                };
                EventHandler rClosed = null;
                rClosed = delegate
                {
                    rDialog.HyperlinkClicked -= rHyperlinkClicked;
                    rDialog.Closed -= rClosed;
                };
                rDialog.HyperlinkClicked += rHyperlinkClicked;
                rDialog.Closed += rClosed;
            }

            rDialog.ShowAndDispose();
        }

        private void EnsureSslCert()
        {
            if (!Preference.Instance.Network.SslCert.Value.IsNullOrEmpty())
                return;

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Product_Name,
                Instruction = StringResources.Instance.Main.Startup_SslCert_Instruction,
                Icon = TaskDialogIcon.Information,
                Buttons =
                    {
                        new TaskDialogCommandLink(TaskDialogCommonButton.Yes, StringResources.Instance.Main.Startup_SslCert_Install),
                        new TaskDialogCommandLink(TaskDialogCommonButton.No, StringResources.Instance.Main.Startup_SslCert_Skip),
                    },
                DefaultCommonButton = TaskDialogCommonButton.Yes,
            };

            if (rDialog.ShowAndDispose().ClickedCommonButton == TaskDialogCommonButton.No)
                return;

            if (CertMaker.rootCertExists())
                return;

            if (!CertMaker.createRootCert())
                return;

            if (!CertMaker.trustRootCert())
                return;

            var cert = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
            var key = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);

            Preference.Instance.Network.SslCert.Value = cert;
            Preference.Instance.Network.SslKey.Value = key;
        }
    }
}
