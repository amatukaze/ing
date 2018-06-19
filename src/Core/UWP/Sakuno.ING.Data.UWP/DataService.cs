using System.IO;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Windows.Storage;

namespace Sakuno.ING.Data.UWP
{
    [Export(typeof(IDataService))]
    internal class DataService : IDataService
    {
        private readonly StorageFolder dataRoot = ApplicationData.Current.LocalFolder;

        public DbContextOptions<TContext> ConfigureDbContext<TContext>(string name)
            where TContext : DbContext
            => new DbContextOptionsBuilder<TContext>()
                .UseSqlite("Data Source=" + Path.Combine(dataRoot.Path, "ing.db"),
                    s => s.MigrationsHistoryTable("__Migration_" + name))
                .Options;
    }
}
