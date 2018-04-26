using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Sakuno.ING.UWP.Bridge
{
    static class Program
    {
        public static NotifyIcon Notify;
        public static Worker Worker;
        [STAThread]
        static void Main(string[] args)
        {
            Notify = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sakuno.ING.UWP.Bridge.app.ico")),
                Text = "ING UWP Proxy",
                Visible = true
            };
            Notify.Click += ShowWindow;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Worker = new Worker();
            ShowWindow(null, null);
            Worker.Start();
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
            new MainWindow { DataContext = Worker }.Show();
        }
    }
}
