using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(BrowserSetting))]
    public class BrowserSetting
    {
        public BrowserSetting(ISettingsManager manager)
        {
            BrowserEngine = manager.Register<string>("browser_engine");
            DefaultUrl = manager.Register("browser.game_url", ConstantUrl);
            Debug = manager.Register("browser.debug_data", false);
        }

        public ISettingItem<string> BrowserEngine { get; }
        public ISettingItem<string> DefaultUrl { get; }
        public ISettingItem<bool> Debug { get; }

        public const double Width = 1200, Height = 720;
        public const string ConstantUrl = "http://www.dmm.com/netgame_s/kancolle/";
        // language=js
        public const string StyleSheetSetJs =
@"if (document.getElementById('game_frame') && !document.getElementById('ING_CUSTOM_STYLE')) {
var sheet = document.createElement('style');
sheet.innerHTML = '#game_frame { position: fixed; left: 0; top: -16px; z-index: 255; } body { margin: 0; overflow: hidden; }';
sheet.id = 'ING_CUSTOM_STYLE';
document.body.appendChild(sheet);
}";
        // language=js
        public const string StyleSheetUnsetJs =
@"var style = document.getElementById('ING_CUSTOM_STYLE');
if (style) {
    style.remove();
}";
    }
}
