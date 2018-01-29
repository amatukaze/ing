using Sakuno.KanColle.Amatsukaze.Data.Localization;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface ILocalizationService
    {
        string GetLocalized(LocalizationCategory category, string id);
    }
}
