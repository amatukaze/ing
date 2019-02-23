using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sakuno.ING.Game.Logger
{
    class LoggerContextFactory : IDesignTimeDbContextFactory<LoggerContext>
    {
        public LoggerContext CreateDbContext(string[] args)
            => new LoggerContext(new DbContextOptionsBuilder<LoggerContext>()
                .UseSqlite("DataSource=" + Path.GetTempFileName())
                .Options);
    }
}
