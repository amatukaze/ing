using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sakuno
{
    public static class TaskExtensions
    {
        public static void WaitAll(this Task[] rpTasks) => Task.WaitAll(rpTasks);
        public static void WaitAny(this Task[] rpTasks) => Task.WaitAny(rpTasks);

        public static Task WhenAll(this IEnumerable<Task> rpTasks) => Task.WhenAll(rpTasks);
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> rpTasks) => Task.WhenAll(rpTasks);

        public static Task<Task> WhenAny(this IEnumerable<Task> rpTasks) => Task.WhenAny(rpTasks);
        public static Task<Task<T>> WhenAny<T>(this IEnumerable<Task<T>> rpTasks) => Task.WhenAny(rpTasks);
    }
}
