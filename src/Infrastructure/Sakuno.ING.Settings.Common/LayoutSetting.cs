using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(LayoutSetting))]
    public class LayoutSetting
    {
        public LayoutSetting(ISettingsManager manager)
        {
            XamlString = manager.Register("application.layout", string.Empty);
            LayoutScale = manager.Register("application.layout_scale", 1f);
            BrowserScale = manager.Register("application.layout_scale_browser", 1f);
        }

        public ISettingItem<string> XamlString { get; }
        public ISettingItem<float> LayoutScale { get; }
        public ISettingItem<float> BrowserScale { get; }
    }
}
