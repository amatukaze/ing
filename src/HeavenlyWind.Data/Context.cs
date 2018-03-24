using System;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    public abstract class Context : DbContext
    {
        internal static IDataService DataService;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DataService == null)
                throw new InvalidOperationException("Data service not initialized.");

            DataService.Configure(optionsBuilder);
        }

        protected Context() => Database.Migrate();
    }
}
