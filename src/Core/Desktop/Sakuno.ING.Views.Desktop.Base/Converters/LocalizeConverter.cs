using System;
using System.Globalization;
using System.Windows.Data;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Views.Desktop.Converters
{
    public sealed class LocalizeConverter : IValueConverter
    {
        public string StringFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(StringFormat)) return null;
            var split = string.Format(StringFormat, value).Split('/');
            return Compositor.Static<ILocalizationService>().GetLocalized(split[0], split[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
