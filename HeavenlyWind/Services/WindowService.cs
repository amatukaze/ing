using System;
using System.Linq;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class WindowService : ModelBase
    {
        public static WindowService Instance { get; } = new WindowService();

        WindowService() { }

        public void Show<T>(object rpDataContext = null, bool rpClearDataContextOnWindowClosed = true) where T : Window, new()
        {
            var rWindow = App.Current.Windows.OfType<T>().SingleOrDefault();
            if (rWindow == null)
            {
                rWindow = new T();
                rWindow.Show();

                if (rpClearDataContextOnWindowClosed)
                    rWindow.Closed += Window_Closed;
            }

            if (rpDataContext != null)
                rWindow.DataContext = rpDataContext;

            rWindow.Activate();

            if (rWindow.WindowState == WindowState.Minimized)
                rWindow.WindowState = WindowState.Normal;
        }

        void Window_Closed(object sender, EventArgs e)
        {
            var rWindow = (Window)sender;

            rWindow.Closed -= Window_Closed;

            var rDisposable = rWindow.DataContext as IDisposable;
            if (rDisposable != null)
                rDisposable.Dispose();

            rWindow.DataContext = null;
        }
    }
}
