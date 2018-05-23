using System.Collections.Generic;
using System.Globalization;

namespace Sakuno.ING.Services
{
    public interface ILocalizationService
    {
        IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
        string GetLocalized(string category, string id);
        string GetUnlocalized(string category, string id);
    }
}
