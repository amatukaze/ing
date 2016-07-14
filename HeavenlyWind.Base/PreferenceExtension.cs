using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class PreferenceExtension : MarkupExtension
    {
        string r_Path;

        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        public ValidationRule ValidationRule { get; set; }

        public PreferenceExtension(string rpPath)
        {
            r_Path = rpPath + ".Value";
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rBinding = new Binding(r_Path) { Source = Preference.Current, Mode = BindingMode.TwoWay, Converter = Converter, ConverterParameter = ConverterParameter, UpdateSourceTrigger = UpdateSourceTrigger };
            if (ValidationRule != null)
                rBinding.ValidationRules.Add(ValidationRule);

            return rBinding.ProvideValue(rpServiceProvider);
        }
    }
}
