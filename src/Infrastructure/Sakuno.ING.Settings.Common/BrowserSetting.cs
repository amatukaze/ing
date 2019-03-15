using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(BrowserSetting))]
    public class BrowserSetting
    {
        public BrowserSetting(ISettingsManager manager)
        {
            BrowserEngine = manager.Register<string>("browser_engine");
            DefaultUrl = manager.Register("browser.game_url", "http://www.dmm.com/netgame_s/kancolle/");
            Debug = manager.Register("browser.debug_data", false);
        }

        public ISettingItem<string> BrowserEngine { get; }
        public ISettingItem<string> DefaultUrl { get; }
        public ISettingItem<bool> Debug { get; }
    }
}
