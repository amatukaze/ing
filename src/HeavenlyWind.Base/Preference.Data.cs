using Sakuno.KanColle.Amatsukaze.Models.Preferences;

namespace Sakuno.KanColle.Amatsukaze
{
    partial class Preference
    {
        public Property<string> Language { get; } = new Property<string>("main.language", StringResources.Instance.GetDefaultLanguage().Directory);

        public Property<string> ExtraResourceLanguage { get; private set; } = new Property<string>("main.language_extra", StringResources.Instance.GetDefaultLanguage().Directory);

        public Property<bool> FirstRun { get; } = new Property<bool>("main.first_run", true);

        public UpdatePreference Update { get; } = new UpdatePreference();

        public NetworkPreference Network { get; } = new NetworkPreference();

        public UserInterfacePreference UI { get; } = new UserInterfacePreference();

        public GamePreference Game { get; } = new GamePreference();

        public CachePreference Cache { get; } = new CachePreference();

        public BrowserPreference Browser { get; } = new BrowserPreference();

        public NotificationPreference Notification { get; } = new NotificationPreference();

        public WindowsPreference Windows { get; } = new WindowsPreference();

        public OtherPreference Other { get; } = new OtherPreference();
    }
}
