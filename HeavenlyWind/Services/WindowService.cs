using System.Linq;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class WindowService : ModelBase
    {
        public static WindowService Instance { get; } = new WindowService();

        WindowService() { }

        public void Show<T>() where T : Window, new()
        {
            var rWindow = App.Current.Windows.OfType<T>().SingleOrDefault();
            if (rWindow == null)
            {
                rWindow = new T();
                rWindow.Show();
            }

            rWindow.Activate();

            if (rWindow.WindowState == WindowState.Minimized)
                rWindow.WindowState = WindowState.Normal;
        }
    }
}
