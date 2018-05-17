using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class ExtraStringResourceExtension : MarkupExtension
    {
        ExtraStringResourceType r_Type;
        string r_IDPath;
        string r_OriginalTextPath;

        public string StringFormat { get; set; }

        public ExtraStringResourceExtension(ExtraStringResourceType rpType, string rpIDPath, string rpOriginalTextPath)
        {
            r_Type = rpType;
            r_IDPath = rpIDPath;
            r_OriginalTextPath = rpOriginalTextPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rResult = new MultiBinding() { Mode = BindingMode.OneWay, Converter = Converter.Instance, ConverterParameter = r_Type, StringFormat = StringFormat };
            rResult.Bindings.Add(new Binding(r_IDPath));
            rResult.Bindings.Add(new Binding(r_OriginalTextPath));
            rResult.Bindings.Add(new Binding(nameof(StringResources.Instance.Extra)) { Source = StringResources.Instance });

            return rResult.ProvideValue(rpServiceProvider);
        }

        class Converter : IMultiValueConverter
        {
            public static Converter Instance { get; } = new Converter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValues[0] == DependencyProperty.UnsetValue || rpValues[1] == DependencyProperty.UnsetValue)
                    return string.Empty;

                var rType = (ExtraStringResourceType)rpParameter;
                var rID = (int)rpValues[0];
                var rOriginalText = rpValues[1];
                var rESR = (ExtraStringResources)rpValues[2];

                if (rESR == null)
                    return rOriginalText;

                var rTranslations = rESR.GetTranslations(rType);
                if (rTranslations == null)
                    return rOriginalText;

                string rTranslatedText;
                if (!rTranslations.TryGetValue(rID, out rTranslatedText))
                    return rOriginalText;

                return rTranslatedText;
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
