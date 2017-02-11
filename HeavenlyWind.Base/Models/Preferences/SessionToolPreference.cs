namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class SessionToolPreference : ModelBase
    {
        public Property<bool> StartRecordingOnAppStartup { get; } = new Property<bool>("other.session_tool.start_recording_on_app_startup");
    }
}
