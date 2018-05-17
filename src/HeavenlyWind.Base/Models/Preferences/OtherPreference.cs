namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class OtherPreference : ModelBase
    {
        public PanicKeyPreference PanicKey { get; } = new PanicKeyPreference();

        public SessionToolPreference SessionTool { get; } = new SessionToolPreference();
    }
}
