using System;

namespace Sakuno.ING.Timing
{
    public interface ITimingService
    {
        DateTimeOffset Now { get; }
        event Action<DateTimeOffset> Elapsed;
    }
}
