using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class ShipSlot : Control
    {
        static ShipSlot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShipSlot), new FrameworkPropertyMetadata(typeof(ShipSlot)));
        }
    }
}
