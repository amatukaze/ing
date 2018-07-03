using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Sakuno.ING
{
    public sealed class ImmutableBindableCollection<T> : ReadOnlyCollection<T>, IBindableCollection<T>
    {
        public ImmutableBindableCollection() : base(Array.Empty<T>()) { }
        public ImmutableBindableCollection(IList<T> source) : base(source) { }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { }
            remove { }
        }
    }

    public static class ImmutableCollectionExtension
    {
        public static ImmutableBindableCollection<T> ToBindable<T>(this IEnumerable<T> source)
            => new ImmutableBindableCollection<T>(source as IList<T> ?? source.ToList());
    }
}
