using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NotificationPreference
    {
        [JsonProperty("expedition")]
        public bool Expedition { get; set; } = true;

        [JsonProperty("repair")]
        public bool Repair { get; set; } = true;

        [JsonProperty("construction")]
        public bool Construction { get; set; } = true;

        [JsonProperty("heavily_damaged")]
        public bool HeavilyDamagedWarning { get; set; } = true;

        [JsonProperty("sound")]
        public NotificationSound Sound { get; set; } = NotificationSound.SystemSound;
        [JsonProperty("sound_filename")]
        public string SoundFilename { get; set; }
    }
}
