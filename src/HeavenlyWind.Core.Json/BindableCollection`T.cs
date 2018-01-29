using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class BindableCollection<T> : IBindableCollection<T>
    {
        IList<T> _list;

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        bool IList.IsReadOnly => true;
        bool IList.IsFixedSize => true;

        object ICollection.SyncRoot => throw new NotSupportedException();
        bool ICollection.IsSynchronized => throw new NotSupportedException();

        T IReadOnlyList<T>.this[int index] => _list[index];
        object IList.this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        public BindableCollection(IList<T> list) => _list = list;

        event PropertyChangedEventHandler IBindableCollection<T>.ChildrenPropertyChanged
        {
            add { }
            remove { }
        }

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

        public bool Contains(T item) => _list.Contains(item);

        public int IndexOf(T item) => _list.IndexOf(item);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        int IList.Add(object value) => throw new NotSupportedException();
        void IList.Remove(object value) => throw new NotSupportedException();
        void IList.Clear() => throw new NotSupportedException();
        void IList.Insert(int index, object value) => throw new NotSupportedException();
        void IList.RemoveAt(int index) => throw new NotSupportedException();

        int IList.IndexOf(object value) => _list.IndexOf((T)value);
        bool IList.Contains(object value) => _list.Contains((T)value);

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
    }
}
