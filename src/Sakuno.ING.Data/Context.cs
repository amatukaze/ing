using System;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    public abstract class Context : DbContext
    {
        internal static IDataService DataService;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DataService == null)
                throw new InvalidOperationException("Data service not initialized.");

            DataService.ConfigureDbContext(optionsBuilder);
        }

        protected Context() => Database.Migrate();
    }
}
