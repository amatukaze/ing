using Sakuno.ING.Game.Models;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed partial class ShipSlotPresenter : UserControl
    {
        public ShipSlotPresenter()
        {
            this.InitializeComponent();
        }

        private HomeportSlot _slot;
        public HomeportSlot Slot
        {
            get => _slot;
            set
            {
                _slot = value;
                Bindings.Update();
            }
        }

        public static string SelectImprovementText(int level) => level switch
        {
            0 => null,
            10 => "max",
            _ => "★" + level
        };

        public static Color SelectAircraftColor(bool isPlane, ClampedValue aircraft)
        {
            if (!isPlane) return Colors.Gray; // not plane
            else if (aircraft.IsMaximum) return Colors.SpringGreen; // full
            else if (aircraft.Current == 0) return Colors.Red; // empty
            else return Colors.YellowGreen; // half
        }
    }
}
