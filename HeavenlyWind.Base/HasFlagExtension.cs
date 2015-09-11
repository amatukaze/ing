using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class HasFlagExtension : MarkupExtension
    {
        string r_Path;
        string r_Parameter;

        public HasFlagExtension(string rpPath, string rpParameter)
        {
            r_Path = rpPath;
            r_Parameter = rpParameter;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = HasFlagConverter.Instance, ConverterParameter = r_Parameter }.ProvideValue(rpServiceProvider);

        class HasFlagConverter : IValueConverter
        {
            public static HasFlagConverter Instance { get; } = new HasFlagConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                var rEnum = rpValue as Enum;
                var rParameter = rpParameter as string;
                if (rEnum != null && rpParameter != null)
                    return rParameter.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => Enum.Parse(rEnum.GetType(), r.Trim()) as Enum).All(rEnum.HasFlag);

                return false;
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }

    }
}
