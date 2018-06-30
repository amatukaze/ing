using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sakuno.ING.Views.Desktop.Converters
{
    internal static class VisibilityBox
    {
        public static object Visible = Visibility.Visible;
        public static object Collapsed = Visibility.Collapsed;
    }

    public sealed class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? VisibilityBox.Visible : VisibilityBox.Collapsed;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
