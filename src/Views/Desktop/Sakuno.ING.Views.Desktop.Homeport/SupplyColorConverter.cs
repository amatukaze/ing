using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Homeport
{
    internal class SupplyColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush full, much, half, empty;
        static SupplyColorConverter()
        {
            full = new SolidColorBrush(Color.FromRgb(57, 255, 0));
            full.Freeze();
            much = new SolidColorBrush(Color.FromRgb(255, 222, 0));
            much.Freeze();
            half = new SolidColorBrush(Color.FromRgb(255, 114, 0));
            half.Freeze();
            empty = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            empty.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (ClampedValue)value;
            if (v.IsMaximum)
                return full;
            else if (v.Current * 9 >= v.Max * 7)
                return much;
            else if (v.Current * 3 >= v.Max)
                return half;
            else
                return empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
