using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class CollapsedIfNullExtension : MarkupExtension
    {
        string r_Path;

        public CollapsedIfNullExtension() { }
        public CollapsedIfNullExtension(string rpPath)
        {
            r_Path = rpPath;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) =>
            new Binding(r_Path) { Converter = NullToVisibilityConverter.Instance }.ProvideValue(rpServiceProvider);

        class NullToVisibilityConverter : IValueConverter
        {
            public static NullToVisibilityConverter Instance { get; } = new NullToVisibilityConverter();

            public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture) => rpValue == null ? Visibility.Collapsed : Visibility.Visible;

            public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
