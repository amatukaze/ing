using Sakuno.ING.Settings;

namespace Sakuno.ING.ViewModels.Layout
{
    public class LayoutSetting
    {
        public LayoutSetting(ISettingsManager manager)
        {
            XmlString = manager.Register("application_layout", string.Empty);
        }

        public ISettingItem<string> XmlString { get; }
    }
}
