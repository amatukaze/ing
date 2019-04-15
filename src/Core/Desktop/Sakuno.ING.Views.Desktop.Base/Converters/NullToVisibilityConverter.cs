using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.ING.Views.Desktop.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is null ? VisibilityBox.Collapsed : VisibilityBox.Visible;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
