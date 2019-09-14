using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
{
    [Export(typeof(ISettingsManager), LazyCreate = false)]
    public class SettingsManager : ISettingsManager
    {
        private readonly DbContextOptions<SettingsDbContext> options;
        private readonly Dictionary<string, SettingItemBase> items = new Dictionary<string, SettingItemBase>();

        public SettingsManager(IDataService dataService)
        {
            options = dataService.ConfigureDbContext<SettingsDbContext>("settings");

            using var context = CreateDbContext();
            context.Database.Migrate();
        }

        internal SettingsDbContext CreateDbContext() => new SettingsDbContext(options);

        public ISettingItem<T> Register<T>(string name, T defaultValue = default)
        {
            lock (items)
            {
                if (items.TryGetValue(name, out var item))
                    return (ISettingItem<T>)item;

                var newItem = new SettingItem<T>(this, name, defaultValue);
                items[name] = newItem;
                return newItem;
            }
        }
    }
}
