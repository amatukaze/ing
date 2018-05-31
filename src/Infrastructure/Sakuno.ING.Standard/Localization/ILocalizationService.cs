using System.Collections.Generic;
using System.Globalization;

namespace Sakuno.ING.Localization
{
    public interface ILocalizationService
    {
        IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
        string GetLocalized(string category, string id);
        string GetUnlocalized(string category, string id);
    }
}
