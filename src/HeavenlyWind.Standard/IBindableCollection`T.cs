using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sakuno.ING
{
    public interface IBindableCollection<out T> : IReadOnlyList<T>, IList, IBindable, INotifyCollectionChanged
    {
    }

    public static class BindableCollectionExtensions
    {
        public static IReadOnlyList<T> AsList<T>(this IBindableCollection<T> source) => source;
    }
}
