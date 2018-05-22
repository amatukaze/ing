using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Data;
using Windows.Storage;

namespace Sakuno.ING.UWP.Data
{
    internal class DataService : IDataService
    {
        private readonly StorageFolder dataRoot = ApplicationData.Current.RoamingFolder;

        public DbContextOptions<TContext> ConfigureDbContext<TContext>(string name)
            where TContext : DbContext
            => new DbContextOptionsBuilder<TContext>()
                .UseSqlite("Data Source=" + Path.Combine(dataRoot.Path, "ing.db"),
                    s => s.MigrationsHistoryTable("__Migration_" + name))
                .Options;

        public Task<Stream> ReadFile(string filename)
            => dataRoot.OpenStreamForReadAsync(filename);

        public Task<Stream> WriteFile(string filename)
            => dataRoot.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);
    }
}
