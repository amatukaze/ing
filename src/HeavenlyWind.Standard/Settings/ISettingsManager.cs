namespace Sakuno.KanColle.Amatsukaze.Settings
{
    public interface ISettingsManager
    {
        ISettingItem<T> Register<T>(string name, T defaultValue = default);
    }
}
