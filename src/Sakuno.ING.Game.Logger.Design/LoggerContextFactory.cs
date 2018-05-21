using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sakuno.ING.Data;

namespace Sakuno.ING.Game.Logger
{
    class LoggerContextFactory : IDesignTimeDbContextFactory<LoggerContext>
    {
        public LoggerContext CreateDbContext(string[] args) => new LoggerContext(new TempDataService());
    }

    internal class TempDataService : IDataService
    {
        public void ConfigureDbContext(string name, DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("DataSource=" + Path.GetTempFileName());
        }
        public Task<Stream> ReadFile(string filename) => throw new NotSupportedException();
        public Task<Stream> WriteFile(string filename) => throw new NotSupportedException();
    }
}
