using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Localization.Embed
{
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
                CultureInfo.CurrentCulture = culture = new CultureInfo(setting.Language.Value);

            while (!string.IsNullOrEmpty(culture.Name))
            {
                using (var stream = asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.{culture.Name}.json"))
                    if (stream != null)
                    {
                        _currentCulture = LoadStrings(stream);
                        break;
                    }
                culture = culture.Parent;
            }

            _fallback = LoadStrings(asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.en-US.json"));
            _gameContent = LoadStrings(asm.GetManifestResourceStream(typeof(LocalizationService), $"Strings.ja.json"));
            if (_currentCulture == null)
                _currentCulture = _fallback;
        }

        public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

        private static Dictionary<string, Dictionary<string, string>> LoadStrings(Stream stream)
            => new JsonSerializer().Deserialize<Dictionary<string, Dictionary<string, string>>>
                (new JsonTextReader(new StreamReader(stream)));

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
