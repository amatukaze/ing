using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;

namespace Sakuno.KanColle.Amatsukaze
{
    partial class Preference
    {

        [JsonProperty("version")]
        public string Version { get; } = ProductInfo.AssemblyVersionString;

        string r_Language;
        [JsonProperty("language")]
        public string Language
        {
            get { return r_Language; }
            set
            {
                if (r_Language != value)
                {
                    r_Language = value;
                    StringResources.Instance.LoadMainResource(value);
                }
            }
        }

        string r_ExtraResourceLanguage;
        [JsonProperty("language_extra")]
        public string ExtraResourceLanguage
        {
            get { return r_ExtraResourceLanguage; }
            set
            {
                if (r_ExtraResourceLanguage != value)
                {
                    r_ExtraResourceLanguage = value;
                    StringResources.Instance.LoadExtraResource(value);
                }
            }
        }

        [JsonProperty("firstrun")]
        public bool FirstRun { get; set; } = true;

        [JsonProperty("update")]
        public UpdatePreference Update { get; set; } = new UpdatePreference();

        [JsonProperty("network")]
        public NetworkPreference Network { get; set; } = new NetworkPreference();

        [JsonProperty("cache")]
        public CachePreference Cache { get; set; } = new CachePreference();

        [JsonProperty("browser")]
        public BrowserPreference Browser { get; set; } = new BrowserPreference();

        WindowsPreference r_Windows;
        [JsonProperty("windows")]
        public WindowsPreference Windows
        {
            get { return r_Windows ?? (r_Windows = new WindowsPreference()); }
            set { r_Windows = value; }
        }

        [JsonProperty("layout")]
        public LayoutPreference Layout { get; set; } = new LayoutPreference();

        public Preference()
        {
            r_Language = r_ExtraResourceLanguage = StringResources.Instance.GetDefaultLanguage().Directory;
        }
    }
}
