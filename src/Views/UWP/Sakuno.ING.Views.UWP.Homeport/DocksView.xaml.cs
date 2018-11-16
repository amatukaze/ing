using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
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
            this.InitializeComponent();
        }

        private static string BuildingStateText(BuildingDockState state)
        {
            switch (state)
            {
                case BuildingDockState.Empty:
                    return "Empty";
                case BuildingDockState.Locked:
                    return "Locked";
                default:
                    return string.Empty;
            }
        }

        private static string RepairingStateText(RepairingDockState state)
        {
            switch (state)
            {
                case RepairingDockState.Empty:
                    return "Empty";
                case RepairingDockState.Locked:
                    return "Locked";
                default:
                    return string.Empty;
            }
        }
    }
}
