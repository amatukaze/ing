namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BrowserPreference : ModelBase
    {
        public Property<double> Zoom { get; } = new Property<double>("browser.zoom", 1.0);

        public Property<string> Homepage { get; } = new Property<string>("browser.homepage", "http://www.dmm.com/netgame_s/kancolle/");

        public Property<string> CurrentLayoutEngine { get; } = new Property<string>("browser.layout_engine", "trident");

        public BlinkPreference Blink { get; } = new BlinkPreference();

        public ScreenshotPreference Screenshot { get; } = new ScreenshotPreference();

        public Property<bool> ActionBarPlacement { get; } = new Property<bool>("browser.action_bar.placement");

        public Property<bool> ActionBarStickToBrowser { get; } = new Property<bool>("browser.action_bar.stick_to_browser");

        public Property<ConfirmationMode> RefreshConfirmationMode { get; } = new Property<ConfirmationMode>("browser.refresh_confirmation", ConfirmationMode.DuringSortie);
    }
}
