namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NotificationPreference : ModelBase
    {
        public Property<bool> Expedition { get; } = new Property<bool>("notification.expedition", true);

        public Property<bool> Repair { get; } = new Property<bool>("notification.repair", true);

        public Property<bool> Construction { get; } = new Property<bool>("notification.construction", true);

        public Property<bool> HeavyDamageWarning { get; } = new Property<bool>("notification.heavy_damage", true);

        public Property<bool> RecoveryFromFatigue { get; } = new Property<bool>("notification.recovery_from_fatigue", true);

        public Property<bool> AnchorageRepair { get; } = new Property<bool>("notification.anchorage_repair", true);

        public Property<NotificationSound> Sound { get; } = new Property<NotificationSound>("notification.sound", NotificationSound.SystemSound);
        public Property<string> SoundFilename { get; } = new Property<string>("notification.sound_filename");

        public Property<NotificationSound> HeavyDamageWarningSound { get; } = new Property<NotificationSound>("notification.sound_hd", NotificationSound.SystemSound);
        public Property<string> HeavyDamageWarningSoundFilename { get; } = new Property<string>("notification.sound_filename_hd");
    }
}
