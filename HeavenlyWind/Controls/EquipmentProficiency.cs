using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class EquipmentProficiency : Control
    {
        public static readonly DependencyProperty ProficiencyProperty = DependencyProperty.Register(nameof(Proficiency), typeof(int), typeof(EquipmentProficiency), new UIPropertyMetadata(0));
        public int Proficiency
        {
            get { return (int)GetValue(ProficiencyProperty); }
            set { SetValue(ProficiencyProperty, value); }
        }

        static EquipmentProficiency()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentProficiency), new FrameworkPropertyMetadata(typeof(EquipmentProficiency)));
        }
    }
}
