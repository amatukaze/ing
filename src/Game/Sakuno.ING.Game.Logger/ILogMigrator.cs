using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.Game.Logger
{
    public interface ILogMigrator : IIdentifiable<string>
    {
        LogType SupportedTypes { get; }
        ValueTask MigrateAsync(FileSystemInfo source, LoggerContext context, LogType selectedTypes);
        bool RequireFolder { get; }
    }
}
