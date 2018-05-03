using System;

namespace Sakuno.ING.Game
{
    public interface ITableProvider
    {
        ITable<T> TryGetTable<T>();
    }

    public static class ITableProviderExtension
    {
        public static ITable<T> GetTable<T>(this ITableProvider provider)
            => provider.TryGetTable<T>() ?? throw new ArgumentException($"Unknown type {typeof(T)}.");
    }
}
