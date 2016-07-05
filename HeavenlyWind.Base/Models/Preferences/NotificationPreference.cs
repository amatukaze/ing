using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NotificationPreference
    {
        [JsonProperty("expedition")]
        public Property<bool> Expedition { get; private set; } = new Property<bool>(true);

        [JsonProperty("repair")]
        public Property<bool> Repair { get; private set; } = new Property<bool>(true);

        [JsonProperty("construction")]
        public Property<bool> Construction { get; private set; } = new Property<bool>(true);

        [JsonProperty("heavily_damaged")]
        public Property<bool> HeavilyDamagedWarning { get; private set; } = new Property<bool>(true);

        [JsonProperty("recovery_from_fatigue")]
        public Property<bool> RecoveryFromFatigue { get; private set; } = new Property<bool>(true);

        [JsonProperty("anchorage_repair")]
        public Property<bool> AnchorageRepair { get; private set; } = new Property<bool>(true);

        [JsonProperty("sound")]
        public Property<NotificationSound> Sound { get; private set; } = new Property<NotificationSound>(NotificationSound.SystemSound);
        [JsonProperty("sound_filename")]
        public Property<string> SoundFilename { get; private set; } = new Property<string>();

        [JsonProperty("sound_hd")]
        public Property<NotificationSound> HeavilyDamagedWarningSound { get; private set; } = new Property<NotificationSound>(NotificationSound.SystemSound);
        [JsonProperty("sound_filename_hd")]
        public Property<string> HeavilyDamagedWarningSoundFilename { get; private set; } = new Property<string>();
    }
}
