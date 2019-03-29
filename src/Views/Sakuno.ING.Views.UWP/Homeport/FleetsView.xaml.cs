using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI;
using Windows.UI.Xaml.Controls;

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

        public static Color SelectSupplyColor(ClampedValue value)
        {
            if (value.IsMaximum)
                return new Color { R = 57, G = 255, A = 255 };
            else if (value.Current * 9 >= value.Max * 7)
                return new Color { R = 255, G = 222, A = 255 };
            else if (value.Current * 3 >= value.Max)
                return new Color { R = 255, G = 114, A = 255 };
            else
                return new Color { R = 255, A = 255 };
        }
    }
}
