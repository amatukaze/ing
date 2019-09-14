using System;

namespace Sakuno.ING.Data
{
    internal abstract class SettingItemBase : BindableObject
    {
        private readonly SettingsManager manager;
        private readonly string key;
        private string value;

        protected SettingItemBase(SettingsManager manager, string key, string defaultValue)
        {
            this.manager = manager;
            this.key = key;

            using var context = manager.CreateDbContext();
            var entry = context.SettingEntries.Find(key);
            if (entry != null)
                value = entry.Value;
            else
            {
                value = defaultValue;
                context.SettingEntries.Add(new SettingDbEntry
                {
                    Id = key,
                    Value = defaultValue
                });
                context.SaveChanges();
            }
        }

        protected string Value
        {
            get => value;
            set
            {
                if (!string.Equals(value, this.value, StringComparison.Ordinal))
                {
                    this.value = value;
                    OnValueChanged(value);
                    NotifyPropertyChanged();

                    using var context = manager.CreateDbContext();
                    context.SettingEntries.Find(key).Value = value;
                    context.SaveChanges();
                }
            }
        }

        protected abstract void OnValueChanged(string value);
    }
}
