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

        internal static Brush StatusColor(FleetStatus status)
        {
            switch (status)
            {
                case FleetStatus.Empty:
                    return new SolidColorBrush(Colors.Gray);
                case FleetStatus.Ready:
                    return new SolidColorBrush(Colors.SpringGreen);
                case FleetStatus.Sortie:
                case FleetStatus.Warning:
                    return new SolidColorBrush(Colors.Red);
                case FleetStatus.Expedition:
                    return new SolidColorBrush(Colors.Aqua);
                case FleetStatus.Fatigued:
                case FleetStatus.Repairing:
                case FleetStatus.Damaged:
                case FleetStatus.Insufficient:
                    return new SolidColorBrush(Colors.Orange);
                default:
                    return null;
            }
        }
    }
}
