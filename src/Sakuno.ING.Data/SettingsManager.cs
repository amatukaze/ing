using System;
using System.Collections.Generic;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
{
    internal class SettingsManager : ISettingsManager
    {
        public SettingsManager(Context context)
        {
            this.context = context;
        }

        private Dictionary<string, SettingItemBase> items = new Dictionary<string, SettingItemBase>();
        private readonly Context context;

        public ISettingItem<T> Register<T>(string name, T defaultValue = default)
        {
            lock (items)
            {
                if (items.ContainsKey(name))
                    throw new InvalidOperationException("One setting name can be registered only once.");
                var item = new SettingItem<T>(context, name, defaultValue);
                items[name] = item;
                return item;
            }
        }
    }
}
