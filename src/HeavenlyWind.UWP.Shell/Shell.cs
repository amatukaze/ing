using Sakuno.KanColle.Amatsukaze.ViewModels;
using Windows.UI.Xaml;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    internal class Shell : IShell
    {
        public void Run()
        {
            Window.Current.Content = new MainPage(new MainWindowVM());
            Window.Current.Activate();
        }
    }
}
