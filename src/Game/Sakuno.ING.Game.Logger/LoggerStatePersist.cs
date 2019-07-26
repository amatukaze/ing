using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger
{
    [Export(typeof(IStatePersist))]
    public class LoggerStatePersist : IStatePersist
    {
    }
}
