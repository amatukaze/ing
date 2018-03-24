using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sakuno.KanColle.Amatsukaze.Settings;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    internal class SettingsManager : Context, ISettingsManager
    {
        public DbSet<SettingDbEntry> Entries { get; set; }
        private Dictionary<string, SettingItemBase> items = new Dictionary<string, SettingItemBase>();
        public ISettingItem<T> Register<T>(string name, T defaultValue = default)
        {
            lock (items)
            {
                if (items.ContainsKey(name))
                    throw new InvalidOperationException("One setting name can be registered only once.");
                var item = new SettingItem<T>(this, name, defaultValue);
                items[name] = item;
                return item;
            }
        }
    }
}
