using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Sakuno.ING.UWP
{
    [Export(typeof(ILocalizationService), LazyCreate = false)]
    internal class LocalizationService : ILocalizationService
    {
        private readonly ResourceLoader localizedLoader = ResourceLoader.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;
        private readonly ResourceContext unlocalizedContext = new ResourceContext
        {
            Languages = new[] { "ja" }
        };

        public LocalizationService(LocaleSetting localeSetting)
        {
            string value = localeSetting.Language.Value;
            if (!string.IsNullOrEmpty(value))
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture
                    = new CultureInfo(value);
        }

        public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
            = Windows.Globalization.ApplicationLanguages.ManifestLanguages.Select(x => new CultureInfo(x)).ToArray();

        public string GetLocalized(string category, string id) => NullIfEmpty(localizedLoader.GetString($"/{category}/{id}"));
        public string GetUnlocalized(string category, string id) => NullIfEmpty(resourceMap.GetValue($"/{category}/{id}", unlocalizedContext)?.ValueAsString);

        private static string NullIfEmpty(string input) => string.IsNullOrEmpty(input) ? null : input;
    }
}
