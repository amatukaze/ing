using Sakuno.KanColle.Amatsukaze.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainWindowVM ViewModel { get; }
        private readonly Shell shell;
        internal MainPage(MainWindowVM viewModel, Shell shell)
        {
            ViewModel = viewModel;
            this.shell = shell;
            this.InitializeComponent();
        }
    }
}
