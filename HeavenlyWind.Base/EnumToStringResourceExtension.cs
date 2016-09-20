using System;
using System.Globalization;
using System.Windows;
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
        public object TargetNullValue { get; set; }

        public EnumToStringResourceExtension(string rpPath, string rpPrefix)
        {
            Path = rpPath;
            Prefix = rpPrefix;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (!StringResources.Instance.IsLoaded)
                return Path;


            var rResult = new MultiBinding() { Converter = r_Converter, ConverterParameter = Prefix, StringFormat = StringFormat, Mode = Mode, TargetNullValue = TargetNullValue };
            rResult.Bindings.Add(new Binding(Path));
            rResult.Bindings.Add(new Binding(nameof(StringResources.Instance.Main)) { Source = StringResources.Instance });

            return rResult.ProvideValue(rpServiceProvider);
        }

        class Converter : IMultiValueConverter
        {
            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValues[0] == null || rpValues[0] == DependencyProperty.UnsetValue)
                    return string.Empty;

                var rValue = rpValues[0];
                var rType = rValue.GetType();
                if (!rType.IsEnum || !rType.IsEnumDefined(rValue))
                    return string.Empty;

                return StringResources.Instance.Main.GetString($"{rpParameter}_{rValue}");
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
