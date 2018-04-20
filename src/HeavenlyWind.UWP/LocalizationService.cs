using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal class LocalizationService : ILocalizationService
    {
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;
        private readonly ResourceContext resourceContext = new ResourceContext();

        public CultureInfo CurrentCulture
        {
            get => CultureInfo.DefaultThreadCurrentUICulture;
            set
            {
                CultureInfo.DefaultThreadCurrentCulture = value;
                CultureInfo.DefaultThreadCurrentUICulture = value;
                string cultureName = value?.ToString() ?? string.Empty;
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = cultureName;
                resourceContext.Languages = new[] { cultureName };
                CultureChanged?.Invoke();
            }
        }

        public event Action CultureChanged;

        public string GetLocalized(string category, string id) => resourceMap.GetValue($"/{category}/{id}", resourceContext).ValueAsString;
    }
}
