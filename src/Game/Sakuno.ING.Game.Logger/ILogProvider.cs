using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger
{
    public interface ILogProvider<TEntity>
        where TEntity : class, ITimedEntity
    {
        ValueTask<IReadOnlyCollection<TEntity>> GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone);
    }
}
