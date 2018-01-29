using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class BindableCollection<T> : IBindableCollection<T>
    {
        IList<T> _list;

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        bool ICollection<T>.IsReadOnly => true;
        bool IList.IsReadOnly => true;
        bool IList.IsFixedSize => true;

        object ICollection.SyncRoot => throw new NotSupportedException();
        bool ICollection.IsSynchronized => throw new NotSupportedException();

        T IList<T>.this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }
        object IList.this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public BindableCollection(IList<T> list) => _list = list;

        public bool Contains(T item) => _list.Contains(item);

        public int IndexOf(T item) => _list.IndexOf(item);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        protected void NotifyPropertyChanged([CallerMemberName] string property = null) =>
            PropertyChanged?.Invoke(this, EventArgsCache.PropertyChanged.Get(property));
        protected void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(this, e);

        void ICollection<T>.Add(T item) => throw new NotSupportedException();
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
        void ICollection<T>.Clear() => throw new NotSupportedException();
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

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
