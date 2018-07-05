using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.ING.Views.Desktop.Converters
{
    public sealed class DateTimeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((DateTimeOffset)value).LocalDateTime.ToString();
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
