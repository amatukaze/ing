using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class OtherPreference
    {
        [JsonProperty("panic_key")]
        public PanicKeyPreference PanicKey { get; private set; } = new PanicKeyPreference();

        [JsonProperty("session_tool")]
        public SessionToolPreference SessionTool { get; private set; } = new SessionToolPreference();
    }
}
