using System;
using System.Windows.Markup;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Views.Desktop
{
    public class LocalizeExtension : MarkupExtension
    {
        public LocalizeExtension(string key) => Key = key;

        [ConstructorArgument("key")]
        public string Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var split = Key.Split('/');
            return Compositor.Static<ILocalizationService>()?.GetLocalized(split[0], split[1]) ?? Key;
        }
    }
}
