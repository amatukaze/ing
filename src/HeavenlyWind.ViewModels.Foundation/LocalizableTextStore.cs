using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.KanColle.Amatsukaze.Services;

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

        private Dictionary<(string, string), WeakReference<LocalizableText>> stored
            = new Dictionary<(string, string), WeakReference<LocalizableText>>();

        private void CultureChanged()
        {
            lock (stored)
                foreach (var key in stored.Keys.ToArray())
                {
                    var wr = stored[key];
                    if (wr.TryGetTarget(out var text))
                    {
                        (string category, string id) = key;
                        text.Text = localizationService.GetLocalized(category, id);
                    }
                    else
                        stored.Remove(key);
                }
        }

        public LocalizableText GetText(string category, string id)
        {
            lock (stored)
            {
                if (stored.TryGetValue((category, id), out var wr)
                    && wr.TryGetTarget(out var text))
                    return text;

                var newText = new LocalizableText { Text = localizationService.GetLocalized(category, id) };
                stored[(category, id)] = new WeakReference<LocalizableText>(newText);
                return newText;
            }
        }
    }
}
