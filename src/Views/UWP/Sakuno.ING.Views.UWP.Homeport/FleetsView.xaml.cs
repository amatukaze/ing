using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Fleets")]
    public sealed partial class FleetsView : UserControl
    {
        private readonly NavalBase ViewModel;
        public FleetsView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }

        internal static Brush StatusColor(FleetState status)
        {
            switch (status)
            {
                case FleetState.Empty:
                    return new SolidColorBrush(Colors.Gray);
                case FleetState.Ready:
                    return new SolidColorBrush(Colors.SpringGreen);
                case FleetState.Sortie:
                case FleetState.Warning:
                    return new SolidColorBrush(Colors.Red);
                case FleetState.Expedition:
                    return new SolidColorBrush(Colors.Aqua);
                case FleetState.Fatigued:
                case FleetState.Repairing:
                case FleetState.Damaged:
                case FleetState.Insufficient:
                    return new SolidColorBrush(Colors.Orange);
                default:
                    return null;
            }
        }
    }
}
