using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public class EquipmentImprovementPresenter : Control
    {
        public static readonly DependencyProperty LevelProperty
            = DependencyProperty.Register(nameof(Level), typeof(int), typeof(EquipmentImprovementPresenter), new PropertyMetadata(0));

        public int Level
        {
            get => (int)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        static EquipmentImprovementPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentImprovementPresenter), new FrameworkPropertyMetadata(typeof(EquipmentImprovementPresenter)));
        }
    }
}
