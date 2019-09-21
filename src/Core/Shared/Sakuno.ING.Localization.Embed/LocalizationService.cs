using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sakuno.ING.Localization.Embed
{
    [Export(typeof(ILocalizationService), LazyCreate = false)]
    internal class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _currentCulture, _fallback, _gameContent;
        public LocalizationService(LocaleSetting setting)
        {
            var asm = Assembly.GetExecutingAssembly();

            SupportedCultures = asm.GetManifestResourceNames()
                .Select(x => new CultureInfo(Path.GetFileNameWithoutExtension(x).Split('.').Last()))
                .ToArray();

            CultureInfo culture;
            if (string.IsNullOrEmpty(setting.Language.Value))
                culture = CultureInfo.CurrentCulture;
            else
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture
                    = culture = new CultureInfo(setting.Language.Value);

            while (!string.IsNullOrEmpty(culture.Name))
            {
                using var stream = asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.{culture.Name}.json");
                if (stream is object)
                {
                    _currentCulture = LoadStringsAsync(stream).Result;
                    break;
                }
                culture = culture.Parent;
            }

            using var fallbackStream = asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.en-US.json");
            _fallback = LoadStringsAsync(fallbackStream).Result;
            using var gameContentStream = asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.ja.json");
            _gameContent = LoadStringsAsync(gameContentStream).Result;

            if (_currentCulture is null)
                _currentCulture = _fallback;
        }

        public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

        private static async Task<Dictionary<string, Dictionary<string, string>>> LoadStringsAsync(Stream stream)
            => await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(stream);

        private static string TryGet(Dictionary<string, Dictionary<string, string>> dict, string category, string id)
            => dict.TryGetValue(category, out var c)
            && c.TryGetValue(id, out string value)
            ? value : null;

        public string GetLocalized(string category, string id)
            => TryGet(_currentCulture, category, id)
            ?? TryGet(_fallback, category, id);

        public string GetUnlocalized(string category, string id)
            => TryGet(_gameContent, category, id);
    }
}
