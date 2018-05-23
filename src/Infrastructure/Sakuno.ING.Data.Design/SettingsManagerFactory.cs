using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sakuno.ING.Data
{
    internal class SettingsManagerFactory : IDesignTimeDbContextFactory<SettingsDbContext>
    {
        public SettingsDbContext CreateDbContext(string[] args)
            => new SettingsDbContext(new DbContextOptionsBuilder<SettingsDbContext>()
                .UseSqlite("DataSource=" + Path.GetTempFileName())
                .Options);
    }
}
