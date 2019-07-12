using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Docks")]
    public sealed partial class DocksView : UserControl
    {
        private readonly NavalBase ViewModel;
        public DocksView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private void Switching(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            if (sender == buttonBuilding)
            {
                panelBuilding.Visibility = Visibility.Visible;
                panelRepairing.Visibility = Visibility.Collapsed;
            }
            else
            {
                panelBuilding.Visibility = Visibility.Collapsed;
                panelRepairing.Visibility = Visibility.Visible;
            }
        }
    }
}
