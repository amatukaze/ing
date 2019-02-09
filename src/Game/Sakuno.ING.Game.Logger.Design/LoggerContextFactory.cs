using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sakuno.ING.Game.Logger
{
    class LoggerContextFactory : IDesignTimeDbContextFactory<LoggerContextBase>
    {
        public LoggerContextBase CreateDbContext(string[] args)
            => new LoggerContextBase(new DbContextOptionsBuilder<LoggerContextBase>()
                .UseSqlite("DataSource=" + Path.GetTempFileName())
                .Options);
    }
}
