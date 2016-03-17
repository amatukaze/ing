using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UpdatePreference
    {
        [JsonProperty("notification")]
        public UpdateNotificationMode NotificationMode { get; set; } = UpdateNotificationMode.AlwaysShow;

        [JsonProperty("channel")]
        public UpdateChannel UpdateChannel { get; set; } = UpdateChannel.Release;
    }
}
