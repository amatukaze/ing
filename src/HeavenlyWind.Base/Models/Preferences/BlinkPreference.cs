namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BlinkPreference : ModelBase
    {
        public Property<bool> DisableHWA { get; } = new Property<bool>("browser.blink.disable_hwa", false);

        public Property<int> MaxFramerate { get; } = new Property<int>("browser.blink.max_framerate", 30);
    }
}
