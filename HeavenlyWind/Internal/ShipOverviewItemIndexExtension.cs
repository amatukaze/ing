using System;
using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class ShipOverviewItemIndexExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rResult = new MultiBinding() { Converter = CoreConverter.Instance };
            rResult.Bindings.Add(new Binding() { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListView), 1) });
            rResult.Bindings.Add(new Binding());

            return rResult.ProvideValue(rpServiceProvider);
        }

        class CoreConverter : IMultiValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                var rListView = rpValues[0] as ListView;
                var rList = rListView.ItemsSource as IList;

                return (rList.IndexOf(rpValues[1]) + 1).ToString();
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
