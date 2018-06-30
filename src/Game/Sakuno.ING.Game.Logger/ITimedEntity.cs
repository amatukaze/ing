using System;

namespace Sakuno.ING.Game.Logger
{
    public interface ITimedEntity
    {
        DateTimeOffset TimeStamp { get; }
    }
}
