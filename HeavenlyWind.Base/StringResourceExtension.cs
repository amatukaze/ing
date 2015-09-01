using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResourceExtension : MarkupExtension
    {
        string r_Path;

        public StringResourceExtension(string rpPath)
        {
            r_Path = rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            if (!StringResources.Instance.IsLoaded)
                return r_Path;

            return new Binding(r_Path) { Source = StringResources.Instance }.ProvideValue(rpServiceProvider);
        }
    }
}
