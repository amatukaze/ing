using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    using UpdateChannelEnum = UpdateChannel;

    public class UpdatePreference
    {
        [JsonProperty("notification")]
        public Property<UpdateNotificationMode> NotificationMode { get; private set; } = new Property<UpdateNotificationMode>(UpdateNotificationMode.AlwaysShow);

        [JsonProperty("channel")]
        public Property<UpdateChannel> UpdateChannel { get; private set; } = new Property<UpdateChannel>(UpdateChannelEnum.Release);
    }
}
