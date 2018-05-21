using System;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    public abstract class DatabaseContext : DbContext
    {
        private readonly IDataService dataService;

        protected DatabaseContext(IDataService dataService)
        {
            this.dataService = dataService;
            Database.Migrate();
        }

        protected abstract string Name { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (dataService == null)
                throw new InvalidOperationException("Data service not initialized.");

            dataService.ConfigureDbContext(Name, optionsBuilder);
        }
    }
}
