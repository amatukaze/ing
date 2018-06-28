using System;
using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.Game.Logger
{
    public interface ILogMigrator : IIdentifiable<string>
    {
        LogType SupportedTypes { get; }
        ValueTask MigrateAsync(FileSystemInfo source, LoggerContext context, LogType selectedTypes, TimeSpan timeZoneOffset, TimeRange? range);
        bool RequireFolder { get; }
    }
}
