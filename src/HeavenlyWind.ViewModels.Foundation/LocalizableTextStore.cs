using Sakuno.KanColle.Amatsukaze.Services;
using System.Collections.Concurrent;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class LocalizableTextStore
    {
        private ILocalizationService localizationService;
        public LocalizableTextStore(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            localizationService.CultureChanged += CultureChanged;
        }

        private ConcurrentDictionary<(string category, string id), LocalizableText> stored
            = new ConcurrentDictionary<(string, string), LocalizableText>();

        private void CultureChanged()
        {
            foreach (var entry in stored)
            {
                var text = entry.Value;
                (string category, string id) = entry.Key;
                text.Text = localizationService.GetLocalized(category, id);
            }
        }

        public LocalizableText GetText(string category, string id)
            => stored.GetOrAdd((category, id), key => new LocalizableText { Text = localizationService.GetLocalized(key.category, key.id) });
    }
}
