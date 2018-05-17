using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResourceExtension : MarkupExtension
    {
        string r_Path;

        public string StringFormat { get; set; }
        public BindingMode Mode { get; set; } = BindingMode.OneWay;

        public StringResourceExtension(string rpPath)
        {
            if (rpPath.StartsWith("Main."))
                r_Path = rpPath;
            else
                r_Path = "Main." + rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (!StringResources.Instance.IsLoaded)
                return r_Path;

            return new Binding(r_Path) { Source = StringResources.Instance, StringFormat = StringFormat, Mode = Mode }.ProvideValue(rpServiceProvider);
        }
    }
}
