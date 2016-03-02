using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    using ConvertClass = Convert;

    public class CollapsedIfZeroExtension : MarkupExtension
    {
        string r_Path;

        public CollapsedIfZeroExtension() { }
        public CollapsedIfZeroExtension(string rpPath)
        {
            r_Path = rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = IsEqualToZeroToVisibilityConverter.Instance }.ProvideValue(rpServiceProvider);

        class IsEqualToZeroToVisibilityConverter : IValueConverter
        {
            public static IsEqualToZeroToVisibilityConverter Instance { get; } = new IsEqualToZeroToVisibilityConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValue is double)
                    return (double)rpValue == .0 ? Visibility.Collapsed : Visibility.Visible;
                else
                    return ConvertClass.ToInt32(rpValue) == 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
