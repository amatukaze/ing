using System;

namespace Sakuno.ING.Timing
{
    public interface ITimingService : IBindable
    {
        DateTimeOffset Now { get; }
    }
}
