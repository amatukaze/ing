namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    using UpdateChannelEnum = UpdateChannel;

    public class UpdatePreference
    {
        public Property<UpdateNotificationMode> NotificationMode { get; } = new Property<UpdateNotificationMode>("update.notification", UpdateNotificationMode.AlwaysShow);

        public Property<UpdateChannel> UpdateChannel { get; } = new Property<UpdateChannel>("update.channel", UpdateChannelEnum.Release);
    }
}
