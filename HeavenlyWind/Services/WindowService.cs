using System.Linq;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class WindowService : ModelBase
    {
        public static WindowService Instance { get; } = new WindowService();

        WindowService() { }

        public void Show<T>(object rpDataContext = null) where T : Window, new()
        {
            var rWindow = App.Current.Windows.OfType<T>().SingleOrDefault();
            if (rWindow == null)
            {
                rWindow = new T();
                rWindow.Show();
            }

            if (rpDataContext != null)
                rWindow.DataContext = rpDataContext;

            rWindow.Activate();

            if (rWindow.WindowState == WindowState.Minimized)
                rWindow.WindowState = WindowState.Normal;
        }
    }
}
