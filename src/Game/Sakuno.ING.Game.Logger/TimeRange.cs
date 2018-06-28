using System;

namespace Sakuno.ING.Game.Logger
{
    public readonly struct TimeRange
    {
        public TimeRange(DateTimeOffset from, DateTimeOffset to)
        {
            From = from;
            To = to;
        }

        public DateTimeOffset From { get; }
        public DateTimeOffset To { get; }

        public bool Contains(DateTimeOffset time)
            => time >= From && time <= To;
    }
}
