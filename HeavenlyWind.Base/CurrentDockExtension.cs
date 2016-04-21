using Sakuno.UserInterface;
using Sakuno.UserInterface.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class CurrentDockExtension : MarkupExtension
    {
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rResult = new MultiBinding() { Mode = BindingMode.OneWay, Converter = CoreConverter.Instance };
            rResult.Bindings.Add(new Binding() { Path = new PropertyPath(MetroWindow.ScreenOrientationProperty), RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MetroWindow), 1) });
            rResult.Bindings.Add(new Binding("Layout.LandscapeDock") { Source = Preference.Current });
            rResult.Bindings.Add(new Binding("Layout.PortraitDock") { Source = Preference.Current });

            if (Converter != null)
                rResult.ConverterParameter = Tuple.Create(Converter, ConverterParameter);

            return rResult.ProvideValue(rpServiceProvider);
        }

        class CoreConverter : IMultiValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValues[0] == DependencyProperty.UnsetValue || rpValues[1] == DependencyProperty.UnsetValue || rpValues[2] == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;

                var rScreenOrientation = (ScreenOrientation)rpValues[0];
                var rResult = rScreenOrientation == ScreenOrientation.Landscape ? rpValues[1] : rpValues[2];

                var rConverter = rpParameter as Tuple<IValueConverter, object>;
                return rConverter == null ? rResult : rConverter.Item1.Convert(rResult, rpTargetType, rConverter.Item2, rpCulture);
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
