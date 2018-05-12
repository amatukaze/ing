using System;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    internal class Context : DbContext
    {
        internal static IDataService DataService;
        public DbSet<SettingDbEntry> SettingEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DataService == null)
                throw new InvalidOperationException("Data service not initialized.");

            DataService.ConfigureDbContext(optionsBuilder);
        }

        public Context() => Database.Migrate();
    }
}
