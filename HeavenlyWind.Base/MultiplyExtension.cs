using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    using ConvertClass = Convert;

    public class MultiplyExtension : MarkupExtension
    {
        string r_Path;
        double r_Parameter;

        public MultiplyExtension(string rpPath, double rpParameter)
        {
            r_Path = rpPath;
            r_Parameter = rpParameter;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = MultiplyConverter.Instance, ConverterParameter = r_Parameter }.ProvideValue(rpServiceProvider);

        class MultiplyConverter : IValueConverter
        {
            public static MultiplyConverter Instance { get; } = new MultiplyConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                var x = ConvertClass.ToDouble(rpValue);
                var y = ConvertClass.ToDouble(rpParameter);
                return x * y;
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
