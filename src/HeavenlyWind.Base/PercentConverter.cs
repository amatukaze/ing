using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ConvertClass = System.Convert;

namespace Sakuno.KanColle.Amatsukaze
{
    public class PercentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            var percentage = ConvertClass.ToDouble(values[0]) / ConvertClass.ToDouble(values[1]);

            return percentage.ToString("P0");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
