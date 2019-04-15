using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sakuno.ING.Views.UWP.Combat
{
    internal static class VisibilityBox
    {
        public static object Visible = Visibility.Visible;
        public static object Collapsed = Visibility.Collapsed;
    }

    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
            => value is null ? VisibilityBox.Collapsed : VisibilityBox.Visible;
        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
