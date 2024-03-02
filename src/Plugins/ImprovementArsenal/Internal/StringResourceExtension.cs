using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Internal
{
    class StringResourceExtension : MarkupExtension
    {
        string r_Path;

        public StringResourceExtension(string rpPath)
        {
            r_Path = rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(nameof(InternalStringResources.Instance.Strings))
            {
                Source = InternalStringResources.Instance,
                Converter = CoreConverter.Instance,
                ConverterParameter = r_Path,
                Mode = BindingMode.OneWay
            }.ProvideValue(rpServiceProvider);

        class CoreConverter : IValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (InternalStringResources.Instance.Strings != null)
                {
                    var rKey = (string)rpParameter;

                    string rValue;
                    if (InternalStringResources.Instance.Strings.TryGetValue(rKey, out rValue))
                        return rValue;
                }

                return rpValue;
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
