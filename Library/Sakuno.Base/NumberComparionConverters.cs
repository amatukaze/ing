using System;
using System.Globalization;
using System.Windows.Data;
using ConvertType = System.Convert;

namespace Sakuno
{
    public abstract class NumberComparionConverter : IValueConverter
    {
        Func<int, int, bool> r_Func;

        protected NumberComparionConverter(Func<int, int, bool> rpFunc)
        {
            r_Func = rpFunc;
        }

        public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            var rValue = ConvertType.ToInt32(rpValue);
            var rParameter = ConvertType.ToInt32(rpParameter);

            return r_Func(rValue, rParameter);
        }

        public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class IsGreaterThanConverter : NumberComparionConverter
    {
        public IsGreaterThanConverter() : base((x, y) => x > y) { }
    }
    [ValueConversion(typeof(int), typeof(bool))]
    public class IsGreaterThanOrEqualToConverter : NumberComparionConverter
    {
        public IsGreaterThanOrEqualToConverter() : base((x, y) => x >= y) { }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class IsLessThanConverter : NumberComparionConverter
    {
        public IsLessThanConverter() : base((x, y) => x < y) { }
    }
    [ValueConversion(typeof(int), typeof(bool))]
    public class IsLessThanOrEqualToConverter : NumberComparionConverter
    {
        public IsLessThanOrEqualToConverter() : base((x, y) => x <= y) { }
    }
}
