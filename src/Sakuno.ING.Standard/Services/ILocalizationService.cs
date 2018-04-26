namespace Sakuno.ING.Services
{
    public interface ILocalizationService
    {
        string GetLocalized(string category, string id);
        string GetUnlocalized(string category, string id);
    }
}
