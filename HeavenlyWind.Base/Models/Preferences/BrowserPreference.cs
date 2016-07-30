namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BrowserPreference
    {
        public Property<double> Zoom { get; } = new Property<double>("browser.zoom", 1.0);

        public Property<string> Homepage { get; } = new Property<string>("browser.homepage", "http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/");

        public Property<string> CurrentLayoutEngine { get; } = new Property<string>("browser.layout_engine", "trident");

        public FlashPreference Flash { get; } = new FlashPreference();

        public ScreenshotPreference Screenshot { get; } = new ScreenshotPreference();

        public Property<bool> ActionBarPlacement { get; } = new Property<bool>("browser.action_bar.placement");

        public Property<bool> ActionBarStickToBrowser { get; } = new Property<bool>("browser.action_bar.stick_to_browser");
    }
}
