using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class EnumToStringResourceExtension : MarkupExtension
    {
        static Converter r_Converter = new Converter();

        public string Path { get; }
        public string Prefix { get; }

        public string StringFormat { get; set; }
        public BindingMode Mode { get; set; } = BindingMode.OneWay;

        public EnumToStringResourceExtension(string rpPath, string rpPrefix)
        {
            Path = rpPath;
            Prefix = rpPrefix;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (!StringResources.Instance.IsLoaded)
                return Path;

            return new Binding(Path) { Converter = r_Converter, ConverterParameter = Prefix, StringFormat = StringFormat, Mode = Mode }.ProvideValue(rpServiceProvider);
        }

        class Converter : IValueConverter
        {
            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValue == null)
                    return string.Empty;

                var rType = rpValue.GetType();
                if (!rType.IsEnum || !rType.IsEnumDefined(rpValue))
                    return string.Empty;

                return StringResources.Instance.Main.GetString($"{rpParameter}_{rpValue}");
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
