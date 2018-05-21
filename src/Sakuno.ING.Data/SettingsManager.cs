using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
{
    internal class SettingsManager : DatabaseContext, ISettingsManager
    {
        public DbSet<SettingDbEntry> SettingEntries { get; set; }
        protected override string Name => "settings";

        private Dictionary<string, SettingItemBase> items = new Dictionary<string, SettingItemBase>();

        public SettingsManager(IDataService dataService) : base(dataService) { }

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
