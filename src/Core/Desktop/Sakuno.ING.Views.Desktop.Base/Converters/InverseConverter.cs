using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.ING.Views.Desktop.Converters
{
    public sealed class InverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is false ? BooleanBox.True : BooleanBox.False;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is false ? BooleanBox.True : BooleanBox.False;
    }
}
