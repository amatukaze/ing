using System;
using System.Threading;

namespace Sakuno
{
    public static class RandomUtil
    {
        static ThreadLocal<Random> r_InstanceOfCurrentThread = new ThreadLocal<Random>(() => new Random(GetSeedOfCurrentThread()));
        public static Random InstanceOfCurrentThread => r_InstanceOfCurrentThread.Value;

        public static int GetSeedOfCurrentThread() => Thread.CurrentThread.GetHashCode() ^ (int)DateTime.UtcNow.Ticks;
    }
}
