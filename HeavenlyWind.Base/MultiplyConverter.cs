using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.KanColle.Amatsukaze
{
    using ConvertClass = Convert;

    public class MultiplyConverter : IValueConverter
    {
        public static MultiplyConverter Instance { get; } = new MultiplyConverter();

        public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            var x = ConvertClass.ToDouble(rpValue);
            var y = ConvertClass.ToDouble(rpParameter);
            return (int)(x * y);
        }

        public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            throw new NotSupportedException();
        }
    }
}
