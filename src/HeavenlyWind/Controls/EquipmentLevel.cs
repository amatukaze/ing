using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class EquipmentLevel : Control
    {
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(nameof(Level), typeof(int), typeof(EquipmentLevel), new UIPropertyMetadata(0));
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public static readonly DependencyProperty ShowStarProperty = DependencyProperty.Register(nameof(ShowStar), typeof(bool), typeof(EquipmentLevel), new PropertyMetadata(BooleanUtil.True));
        public bool ShowStar
        {
            get { return (bool)GetValue(ShowStarProperty); }
            set { SetValue(ShowStarProperty, value); }
        }

        static EquipmentLevel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentLevel), new FrameworkPropertyMetadata(typeof(EquipmentLevel)));
        }
    }
}
