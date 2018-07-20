using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.ING.Views.Desktop.Converters
{
    internal static class BooleanBox
    {
        public static object True = true;
        public static object False = false;
    }

    public sealed class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null ? BooleanBox.True : BooleanBox.False;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
