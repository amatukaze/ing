using System;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    internal abstract class SettingItemBase : BindableObject
    {
        private SettingsManager manager;
        private SettingDbEntry entry;

        protected SettingItemBase(SettingsManager manager,string key, string defaultValue)
        {
            this.manager = manager;
            entry = manager.SettingEntries.Find(key)
                ?? manager.SettingEntries.Add(new SettingDbEntry
                {
                    Id = key,
                    Value = defaultValue
                }).Entity;
        }

        protected string Value
        {
            get => entry.Value;
            set
            {
                if (!string.Equals(value, entry.Value, StringComparison.Ordinal))
                {
                    entry.Value = value;
                    manager.SaveChanges();
                    OnValueChanged(value);
                    NotifyPropertyChanged();
                }
            }
        }

        protected abstract void OnValueChanged(string value);
    }
}
