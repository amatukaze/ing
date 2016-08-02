namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class OtherPreference
    {
        public PanicKeyPreference PanicKey { get; } = new PanicKeyPreference();

        public SessionToolPreference SessionTool { get; } = new SessionToolPreference();
    }
}
