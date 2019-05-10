using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Controls
{

    public partial class ShipSlotPresenter : UserControl
    {
        public ShipSlotPresenter()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SlotProperty
            = DependencyProperty.Register(nameof(Slot), typeof(Slot), typeof(ShipSlotPresenter), new PropertyMetadata(null));
        public Slot Slot
        {
            get => (Slot)GetValue(SlotProperty);
            set => SetValue(SlotProperty, value);
        }
    }
}
