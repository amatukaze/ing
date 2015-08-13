using System;

namespace Sakuno
{
    public static class DateTimeUtil
    {
        public static DateTimeOffset UnixEpoch { get; } = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static DateTimeOffset FromUnixTime(ulong rpValue) => UnixEpoch.AddSeconds((double)rpValue);

        public static ulong ToUnixTime(this DateTimeOffset rpDateTime) => (ulong)rpDateTime.Subtract(UnixEpoch).TotalSeconds;
    }
}
