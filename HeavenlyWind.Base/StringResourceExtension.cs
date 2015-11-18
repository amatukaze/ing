using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResourceExtension : MarkupExtension
    {
        string r_Path;

        public string StringFormat { get; set; }
        public BindingMode Mode { get; set; }

        public StringResourceExtension(string rpPath)
        {
            r_Path = rpPath;
            Mode = BindingMode.Default;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (!StringResources.Instance.IsLoaded)
                return r_Path;

            return new Binding(r_Path) { Source = StringResources.Instance, StringFormat = StringFormat, Mode = Mode }.ProvideValue(rpServiceProvider);
        }
    }
}
