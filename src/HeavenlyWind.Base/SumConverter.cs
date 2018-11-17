using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sakuno.KanColle.Amatsukaze
{
    public class SumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
                if (value == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;

            var result = 0;

            foreach (var value in values)
                result += (int)value;

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
