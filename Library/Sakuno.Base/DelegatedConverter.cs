using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno
{
    public class DelegatedConverter<TSource, TTarget> : IValueConverter
    {
        protected Func<TSource, object, CultureInfo, TTarget> r_Convert;
        protected Func<TTarget, object, CultureInfo, TSource> r_ConvertBack;

        protected DelegatedConverter() { }
        public DelegatedConverter(Func<TSource, object, CultureInfo, TTarget> rpConvert) : this(rpConvert, null) { }
        public DelegatedConverter(Func<TSource, object, CultureInfo, TTarget> rpConvert, Func<TTarget, object, CultureInfo, TSource> rpConvertBack)
        {
            if (rpConvert == null)
                throw null;

            r_Convert = rpConvert;
            r_ConvertBack = rpConvertBack;
        }

        object IValueConverter.Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            => r_Convert((TSource)rpValue, rpParameter, rpCulture);

        object IValueConverter.ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            if (r_ConvertBack == null)
                throw new NotSupportedException();

            return r_ConvertBack((TTarget)rpValue, rpParameter, rpCulture);
        }
    }
}
