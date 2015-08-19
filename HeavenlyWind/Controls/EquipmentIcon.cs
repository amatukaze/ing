using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class EquipmentIcon : Control
    {
        public EquipmentIconType Type
        {
            get { return (EquipmentIconType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(EquipmentIconType), typeof(EquipmentIcon),
            new UIPropertyMetadata(EquipmentIconType.None));

        static EquipmentIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentIcon), new FrameworkPropertyMetadata(typeof(EquipmentIcon)));
        }
    }
}
