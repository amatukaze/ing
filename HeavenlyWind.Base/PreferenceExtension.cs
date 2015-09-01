using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class PreferenceExtension : MarkupExtension
    {
        string r_Path;

        public PreferenceExtension(string rpPath)
        {
            r_Path = rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) => new Binding(r_Path) { Source = Preference.Current, Mode = BindingMode.TwoWay }.ProvideValue(rpServiceProvider);
    }
}
