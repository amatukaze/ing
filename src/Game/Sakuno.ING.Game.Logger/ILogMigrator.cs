using System.IO;

namespace Sakuno.ING.Game.Logger
{
    public interface ILogMigrator : IIdentifiable<string>
    {
        LogType SupportedTypes { get; }
        bool Migrate(FileSystemInfo source, LoggerContext context, LogType selectedTypes);
    }
}
