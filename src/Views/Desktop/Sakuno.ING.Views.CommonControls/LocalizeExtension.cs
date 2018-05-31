using System;
using System.Windows.Markup;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Views
{
    public class LocalizeExtension : MarkupExtension
    {
        public LocalizeExtension(string key) => Key = key;

        [ConstructorArgument("key")]
        public string Key { get; set; }

        internal static ILocalizationService Service;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var split = Key.Split('/');
            return Service?.GetLocalized(split[0], split[1]) ?? Key;
        }
    }
}
