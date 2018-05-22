using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
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

        public Task<Stream> ReadFile(string filename) => Task.FromResult<Stream>(File.OpenRead(Path.Combine(basePath, filename)));
        public Task<Stream> WriteFile(string filename) => Task.FromResult<Stream>(File.OpenWrite(Path.Combine(basePath, filename)));
    }
}
