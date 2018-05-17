using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class BitMaskExtension : MarkupExtension
    {
        string r_Path;

        int r_Mask;
        int r_Offset;

        public BitMaskExtension(string rpPath, int rpMask, int rpOffset)
        {
            r_Path = rpPath;

            r_Mask = rpMask;
            r_Offset = rpOffset;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = BitMaskConverter.Instance, ConverterParameter = Tuple.Create(r_Mask, r_Offset), Mode = BindingMode.OneWay }.ProvideValue(rpServiceProvider);

        class BitMaskConverter : IValueConverter
        {
            public static BitMaskConverter Instance { get; } = new BitMaskConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                var rParameter = (Tuple<int, int>)rpParameter;
                var rData = (int)rpValue;

                return (rData & rParameter.Item1 << rParameter.Item2) >> rParameter.Item2;
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
