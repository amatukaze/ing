using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class MultiplyExtension : MarkupExtension
    {
        string r_Path;
        double r_Parameter;

        public MultiplyExtension(string rpPath, double rpParameter)
        {
            r_Path = rpPath;
            r_Parameter = rpParameter;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = MultiplyConverter.Instance, ConverterParameter = r_Parameter }.ProvideValue(rpServiceProvider);
    }
}
