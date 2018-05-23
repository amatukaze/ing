namespace Sakuno.ING.Settings
{
    public interface ISettingsManager
    {
        ISettingItem<T> Register<T>(string name, T defaultValue = default);
    }
}
