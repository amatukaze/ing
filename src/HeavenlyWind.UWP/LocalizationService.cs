using System;
using System.Globalization;
using Sakuno.KanColle.Amatsukaze.Services;
using Windows.ApplicationModel.Resources;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal class LocalizationService : ILocalizationService
    {
        private ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        public CultureInfo CurrentCulture
        {
            get => CultureInfo.DefaultThreadCurrentUICulture;
            set
            {
                CultureInfo.DefaultThreadCurrentCulture = value;
                CultureInfo.DefaultThreadCurrentUICulture = value;
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = value.ToString();
                CultureChanged?.Invoke();
            }
        }

        public event Action CultureChanged;

        public string GetLocalized(string category, string id) => resourceLoader.GetString(category + "/" + id);
    }
}
