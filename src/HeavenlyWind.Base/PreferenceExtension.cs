using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class PreferenceExtension : MarkupExtension
    {
        static DependencyObject r_DesignModeDetector = new DependencyObject();

        string r_Path;

        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        public ValidationRule ValidationRule { get; set; }

        public string StringFormat { get; set; }

        public PreferenceExtension(string rpPath)
        {
            r_Path = rpPath + ".Value";
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (DesignerProperties.GetIsInDesignMode(r_DesignModeDetector))
                return DependencyProperty.UnsetValue;

            var rBinding = new Binding(r_Path) { Source = Preference.Instance, Mode = BindingMode.TwoWay, Converter = Converter, ConverterParameter = ConverterParameter, UpdateSourceTrigger = UpdateSourceTrigger, StringFormat = StringFormat };
            if (ValidationRule != null)
                rBinding.ValidationRules.Add(ValidationRule);

            return rBinding.ProvideValue(rpServiceProvider);
        }
    }
}
