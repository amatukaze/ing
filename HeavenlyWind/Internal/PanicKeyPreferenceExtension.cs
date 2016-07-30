using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class PanicKeyPreferenceExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rResult = new MultiBinding() { Mode = BindingMode.OneWay, Converter = CoreConverter.Instance };
            rResult.Bindings.Add(new Binding("ModifierKeys.Value") { Source = Preference.Instance.Other.PanicKey });
            rResult.Bindings.Add(new Binding("Key.Value") { Source = Preference.Instance.Other.PanicKey });

            return rResult.ProvideValue(rpServiceProvider);
        }

        class CoreConverter : IMultiValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                var rBuilder = new StringBuilder(32);

                PanicKeyService.GetModifierKeysString((int)rpValues[0], rBuilder);
                PanicKeyService.GetKeyString(KeyInterop.KeyFromVirtualKey((int)rpValues[1]), rBuilder);

                return rBuilder.ToString();
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
