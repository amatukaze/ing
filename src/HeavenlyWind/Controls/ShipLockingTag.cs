using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class ShipLockingTag : Control
    {
        static ShipLockingTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShipLockingTag), new FrameworkPropertyMetadata(typeof(ShipLockingTag)));
        }
    }
}
