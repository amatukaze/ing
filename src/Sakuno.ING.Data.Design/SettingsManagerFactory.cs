using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sakuno.ING.Data
{
    internal class SettingsManagerFactory : IDesignTimeDbContextFactory<SettingsManager>
    {
        public SettingsManager CreateDbContext(string[] args) => new SettingsManager(new TempDataService());
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
