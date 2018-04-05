using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    static class Program
    {
        public static NotifyIcon Notify;
        static Worker worker;
        [STAThread]
        static void Main(string[] args)
        {
            Notify = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sakuno.KanColle.Amatsukaze.UWP.Bridge.app.ico")),
                Text = "ING UWP Proxy",
                Visible = true
            };
            Notify.Click += ShowWindow;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            worker = new Worker();
            ShowWindow(null, null);
            worker.Start();
            new Application
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            }.Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.IO.File.AppendAllLines("error.log", new[] { e.ExceptionObject.ToString() });
        }

        static void ShowWindow(object sender, object e)
        {
            new MainWindow { DataContext = worker }.Show();
        }
    }
}
