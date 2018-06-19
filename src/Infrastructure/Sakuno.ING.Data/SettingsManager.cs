using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
{
    [Export(typeof(ISettingsManager), LazyCreate = false)]
    internal class SettingsManager : ISettingsManager
    {
        private readonly DbContextOptions<SettingsDbContext> options;
        private Dictionary<string, SettingItemBase> items = new Dictionary<string, SettingItemBase>();

        public SettingsManager(IDataService dataService)
        {
            options = dataService.ConfigureDbContext<SettingsDbContext>("settings");

            using (var context = CreateDbContext())
                context.Database.Migrate();
        }

        public SettingsDbContext CreateDbContext() => new SettingsDbContext(options);

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
