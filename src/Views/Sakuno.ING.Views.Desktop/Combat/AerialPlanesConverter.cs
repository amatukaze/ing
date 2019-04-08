using System;
using System.Globalization;
using System.Windows.Data;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Views.Desktop.Combat
{
    internal class AerialPlanesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (ClampedValue)value;
            return $"{v.Max}→{v.Current} (-{v.Shortage})";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
