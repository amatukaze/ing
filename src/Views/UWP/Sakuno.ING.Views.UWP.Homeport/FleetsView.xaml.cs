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

        private static Brush StatusColor(FleetStatus status)
        {
            switch (status)
            {
                case FleetStatus.Empty:
                    return new SolidColorBrush(Colors.Gray);
                case FleetStatus.Ready:
                    return new SolidColorBrush(Colors.SpringGreen);
                case FleetStatus.Sortie:
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

        private static string StatusText(FleetStatus status)
        {
            switch (status)
            {
                case FleetStatus.Empty:
                    return "Empty fleet";
                case FleetStatus.Ready:
                    return "Ready";
                case FleetStatus.Sortie:
                    return "In sortie";
                case FleetStatus.Expedition:
                    return "In expedition";
                case FleetStatus.Fatigued:
                    return "Low morale";
                case FleetStatus.Repairing:
                    return "Repairing";
                case FleetStatus.Damaged:
                    return "Heavily damaged";
                case FleetStatus.Insufficient:
                    return "Supply required";
                default:
                    return string.Empty;
            }
        }
    }
}
