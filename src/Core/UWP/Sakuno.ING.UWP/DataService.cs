using System.IO;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Windows.Storage;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IDataService))]
    internal class DataService : IDataService
    {
        private readonly StorageFolder dataRoot = ApplicationData.Current.LocalFolder;

        public DbContextOptions<TContext> ConfigureDbContext<TContext>(string filename, string entityname = null)
            where TContext : DbContext
            => new DbContextOptionsBuilder<TContext>()
                .UseSqlite($"DataSource={Path.Combine(dataRoot.Path, filename + ".db")}",
                    s => s.MigrationsHistoryTable("__Migration_" + (entityname ?? "Default")))
                .Options;
    }
}
