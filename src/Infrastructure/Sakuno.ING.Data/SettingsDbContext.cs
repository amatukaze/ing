using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    internal class SettingsDbContext : DbContext
    {
        public DbSet<SettingDbEntry> SettingEntries { get; private set; }

        public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options) { }
    }
}
