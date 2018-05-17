using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class AppIcon : Control
    {
        static AppIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AppIcon), new FrameworkPropertyMetadata(typeof(AppIcon)));
        }
    }
}
