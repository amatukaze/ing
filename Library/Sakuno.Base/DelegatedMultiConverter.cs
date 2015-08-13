using System;
using System.Globalization;
using System.Windows.Data;

namespace Sakuno.UserInterface
{
    using ConvertBackFunc = Func<object, object, CultureInfo, object[]>;
    using ConvertFunc = Func<object[], object, CultureInfo, object>;

    public class DelegatedMultiConverter : IMultiValueConverter
    {
        ConvertFunc r_Convert;
        ConvertBackFunc r_ConvertBack;

        public DelegatedMultiConverter(ConvertFunc rpConvert) : this(rpConvert, null) { }
        public DelegatedMultiConverter(ConvertFunc rpConvert, ConvertBackFunc rpConvertBack)
        {
            if (rpConvert == null)
                throw null;

            r_Convert = rpConvert;
            r_ConvertBack = rpConvertBack;
        }

        object IMultiValueConverter.Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            => r_Convert(rpValues, rpParameter, rpCulture);

        object[] IMultiValueConverter.ConvertBack(object rpValue, Type[] rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            if (r_ConvertBack == null)
                throw new NotSupportedException();

            return r_ConvertBack(rpValue, rpParameter, rpCulture);
        }
    }
}
