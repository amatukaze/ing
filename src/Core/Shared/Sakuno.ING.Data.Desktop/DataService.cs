using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Data.Desktop
{
    [Export(typeof(IDataService))]
    internal class DataService : IDataService
    {
        private readonly string basePath =
            Path.Combine(Environment.CurrentDirectory, "data");

        public DbContextOptions<TContext> ConfigureDbContext<TContext>(string name)
            where TContext : DbContext
            => new DbContextOptionsBuilder<TContext>()
                .UseSqlite($"DataSource={Path.Combine(basePath, "ing.db")}",
                    s => s.MigrationsHistoryTable("__Migration_" + name))
                .Options;
    }
}
