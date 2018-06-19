using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(LayoutSetting))]
    public class LayoutSetting
    {
        public LayoutSetting(ISettingsManager manager)
        {
            XamlString = manager.Register("application_layout", string.Empty);
        }

        public ISettingItem<string> XamlString { get; }
    }
}
