using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class SessionToolPreference
    {
        [JsonProperty("start_recording_on_app_startup")]
        public Property<bool> StartRecordingOnAppStartup { get; private set; } = new Property<bool>();
    }
}
