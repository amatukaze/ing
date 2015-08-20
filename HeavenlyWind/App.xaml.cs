using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.Views;
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

            Preference.Load();
            StringResources.Instance.Load(Preference.Current.Language);

            MainWindow = new MainWindow();
            MainWindow.DataContext = Root = new MainWindowViewModel();
            MainWindow.Show();
        }
    }
}
