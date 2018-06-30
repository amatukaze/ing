using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.Game.Logger
{
    public interface ILogProvider<TEntity>
        where TEntity : class, ITimedEntity
    {
        ValueTask<IReadOnlyCollection<TEntity>> GetLogsAsync(FileSystemInfo source, TimeSpan timeZone);
    }
}
