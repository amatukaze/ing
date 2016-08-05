using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    class CommonPropertyIcon : Control
    {
        public CommonProperty Type
        {
            get { return (CommonProperty)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(CommonProperty), typeof(CommonPropertyIcon),
            new UIPropertyMetadata(EnumUtil.GetBoxed(CommonProperty.None)));

        static CommonPropertyIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommonPropertyIcon), new FrameworkPropertyMetadata(typeof(CommonPropertyIcon)));
        }
    }
}
