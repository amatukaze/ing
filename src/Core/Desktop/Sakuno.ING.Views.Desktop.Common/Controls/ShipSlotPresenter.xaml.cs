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
            = DependencyProperty.Register(nameof(Slot), typeof(HomeportSlot), typeof(ShipSlotPresenter), new PropertyMetadata(null));
        public HomeportSlot Slot
        {
            get => (HomeportSlot)GetValue(SlotProperty);
            set => SetValue(SlotProperty, null);
        }
    }
}
