using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze.Converters
{
    class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var language = (string)value;

            return language switch
            {
                "Japanese" => XmlLanguage.GetLanguage("ja-JP"),
                "SimplifiedChinese" => XmlLanguage.GetLanguage("zh-Hans"),
                "TraditionalChinese" => XmlLanguage.GetLanguage("zh-Hant"),

                _ => XmlLanguage.GetLanguage("en-US"),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
