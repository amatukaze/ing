using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sakuno.ING.Localization;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Sakuno.ING.UWP
{
    internal class LocalizationService : ILocalizationService
    {
        private readonly ResourceLoader localizedLoader = ResourceLoader.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;
        private readonly ResourceContext unlocalizedContext = new ResourceContext
        {
            Languages = new[] { "ja" }
        };

        public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
            = Windows.Globalization.ApplicationLanguages.ManifestLanguages.Select(x => new CultureInfo(x)).ToArray();

        public string GetLocalized(string category, string id) => NullIfEmpty(localizedLoader.GetString($"/{category}/{id}"));
        public string GetUnlocalized(string category, string id) => NullIfEmpty(resourceMap.GetValue($"/{category}/{id}", unlocalizedContext).ValueAsString);

        private static string NullIfEmpty(string input) => string.IsNullOrEmpty(input) ? null : input;
    }
}
