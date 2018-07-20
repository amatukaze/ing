using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Sakuno.ING.Data.Design")]
namespace Sakuno.ING.Data
{
    internal class SettingsDbContext : DbContext
    {
        public DbSet<SettingDbEntry> SettingEntries { get; private set; }

        public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options) { }
    }
}
