using System;
using System.Globalization;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; set; }

        string GetLocalized(string category, string id);

        event Action CultureChanged;
    }
}
