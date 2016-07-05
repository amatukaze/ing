using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;

namespace Sakuno.KanColle.Amatsukaze
{
    partial class Preference
    {

        [JsonProperty("version")]
        public string Version { get; } = ProductInfo.AssemblyVersionString;

        [JsonProperty("language")]
        public Property<string> Language { get; private set; } = new Property<string>(StringResources.Instance.GetDefaultLanguage().Directory);

        [JsonProperty("language_extra")]
        public Property<string> ExtraResourceLanguage { get; private set; } = new Property<string>(StringResources.Instance.GetDefaultLanguage().Directory);

        [JsonProperty("firstrun")]
        public bool FirstRun { get; set; } = true;

        [JsonProperty("update")]
        public UpdatePreference Update { get; private set; } = new UpdatePreference();

        [JsonProperty("network")]
        public NetworkPreference Network { get; private set; } = new NetworkPreference();

        [JsonProperty("ui")]
        public UserInterfacePreference UI { get; private set; } = new UserInterfacePreference();

        [JsonProperty("game")]
        public GamePreference Game { get; private set; } = new GamePreference();

        [JsonProperty("cache")]
        public CachePreference Cache { get; private set; } = new CachePreference();

        [JsonProperty("browser")]
        public BrowserPreference Browser { get; private set; } = new BrowserPreference();

        [JsonProperty("notification")]
        public NotificationPreference Notification { get; private set; } = new NotificationPreference();

        WindowsPreference r_Windows;
        [JsonProperty("windows")]
        public WindowsPreference Windows
        {
            get { return r_Windows ?? (r_Windows = new WindowsPreference()); }
            private set { r_Windows = value; }
        }

        [JsonProperty("layout")]
        public LayoutPreference Layout { get; private set; } = new LayoutPreference();

        [JsonProperty("other")]
        public OtherPreference Other { get; private set; } = new OtherPreference();
    }
}
