using Windows.UI.Xaml;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    internal class Shell : IShell
    {
        public void Run()
        {
            Window.Current.Content = new MainPage();
            Window.Current.Activate();
        }
    }
}
