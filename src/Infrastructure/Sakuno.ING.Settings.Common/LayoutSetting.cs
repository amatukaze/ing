namespace Sakuno.ING.Settings
{
    public class LayoutSetting
    {
        public LayoutSetting(ISettingsManager manager)
        {
            XamlString = manager.Register("application_layout", string.Empty);
        }

        public ISettingItem<string> XamlString { get; }
    }
}
