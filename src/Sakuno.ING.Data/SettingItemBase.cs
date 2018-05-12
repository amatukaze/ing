using System;

namespace Sakuno.ING.Data
{
    internal abstract class SettingItemBase : BindableObject
    {
        private Context context;
        private SettingDbEntry entry;

        protected SettingItemBase(Context context,string key, string defaultValue)
        {
            this.context = context;
            entry = context.SettingEntries.Find(key)
                ?? context.SettingEntries.Add(new SettingDbEntry
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
                    context.SaveChanges();
                    OnValueChanged(value);
                    NotifyPropertyChanged();
                }
            }
        }

        protected abstract void OnValueChanged(string value);
    }
}
