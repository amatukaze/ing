using Sakuno.KanColle.Amatsukaze.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal sealed partial class MainView : Page
    {
        private readonly MainWindowVM ViewModel;
        internal MainView(MainWindowVM viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
