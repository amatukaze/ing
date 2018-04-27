using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    internal class DataService : IDataService
    {
        private readonly string basePath = Path.Combine
        (
            Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().CodeBase),
            "data"
        );

        public void ConfigureDbContext(DbContextOptionsBuilder builder)
            => builder.UseSqlite($"DataSource={Path.Combine(basePath, "ing.db")}");

        public Task<Stream> ReadFile(string filename) => Task.FromResult<Stream>(File.OpenRead(Path.Combine(basePath, filename)));
        public Task<Stream> WriteFile(string filename) => Task.FromResult<Stream>(File.OpenWrite(Path.Combine(basePath, filename)));
    }
}
