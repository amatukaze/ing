using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class MaterialIcon : Control
    {
        public MaterialType Type
        {
            get { return (MaterialType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(MaterialType), typeof(MaterialIcon),
            new UIPropertyMetadata(MaterialType.None));

        static MaterialIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaterialIcon), new FrameworkPropertyMetadata(typeof(MaterialIcon)));
        }
    }
}
