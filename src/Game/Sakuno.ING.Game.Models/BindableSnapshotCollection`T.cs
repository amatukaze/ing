using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Sakuno.ING.Game
{
    public partial class BindableSnapshotCollection<T> : IDisposable, IBindableCollection<T>
    {
        private readonly IUpdationSource source;
        private List<T> snapshot = new List<T>();

        private IEnumerable<T> _query;
        public IEnumerable<T> Query
        {
            get => _query;
            set
            {
                _query = value ?? throw new ArgumentNullException(nameof(Query));
                Refresh();
            }
        }

        public BindableSnapshotCollection() { }

        public BindableSnapshotCollection(IUpdationSource source, IEnumerable<T> query)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            _query = query ?? throw new ArgumentNullException(nameof(query));

            if (BindableObject.ThreadSafeEnabled)
                pHandlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();

            snapshot = query.ToList();
            source.Updated += Refresh;
        }

        public void Dispose()
        {
            if (source != null)
                source.Updated -= Refresh;
        }

        #region PropertyChange
        private List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)> pHandlers;
        private PropertyChangedEventHandler pHandler;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (BindableObject.ThreadSafeEnabled)
                    lock (pHandlers)
                        pHandlers.Add((SynchronizationContext.Current, value));
                else
                    pHandler += value;
            }
            remove
            {
                if (BindableObject.ThreadSafeEnabled)
                {
                    lock (pHandlers)
                        for (int i = 0; i < pHandlers.Count; i++)
                            if (pHandlers[i].handler == value)
                                pHandlers.RemoveAt(i--);
                }
                else
                    pHandler -= value;
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var arg = new PropertyChangedEventArgs(propertyName);
            if (BindableObject.ThreadSafeEnabled)
                lock (pHandlers)
                    foreach (var (syncContext, handler) in pHandlers)
                        syncContext.Post(o => handler(this, arg), null);
            else
                pHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region CollectionChange
        private List<(SynchronizationContext syncContext, NotifyCollectionChangedEventHandler handler)>
            cHandlers = new List<(SynchronizationContext, NotifyCollectionChangedEventHandler)>();
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (cHandlers)
                    cHandlers.Add((SynchronizationContext.Current, value));
            }
            remove
            {
                lock (cHandlers)
                    for (int i = 0; i < cHandlers.Count; i++)
                        if (cHandlers[i].handler == value)
                            cHandlers.RemoveAt(i--);
            }
        }
        private void NotifyCollectionChanged(params NotifyCollectionChangedEventArgs[] args)
        {
            lock (cHandlers)
                foreach (var (context, handler) in cHandlers)
                    context.Post(o =>
                    {
                        foreach (var e in args)
                            handler(this, e);
                    }, null);
        }
        #endregion

        public void Refresh()
        {
            var @new = Query.ToList();
            if(!snapshot.SequenceEqual(@new))
            {
                snapshot = @new;
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void Remove(T item) => Remove(snapshot.IndexOf(item), item);

        public void RemoveAt(int index) => Remove(index, snapshot[index]);

        private void Remove(int index, T item)
        {
            if (index >= 0)
            {
                snapshot.RemoveAt(index);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }

        public void Replace(T oldItem, T newItem)
        {
            int i = snapshot.IndexOf(oldItem);

            if (i >= 0)
            {
                snapshot[i] = newItem;
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, i));
            }
        }

        public int Count => snapshot.Count;
        public T this[int index]
        {
            get => snapshot[index];
            set
            {
                if (index > snapshot.Count)
                    throw new IndexOutOfRangeException(nameof(index));
                else if (index == snapshot.Count)
                {
                    snapshot.Add(value);
                    NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
                }
                else
                {
                    var oldValue = snapshot[index];
                    snapshot[index] = value;
                    NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
                }
            }
        }

        #region IEnumerable
        public List<T>.Enumerator GetEnumerator() => snapshot.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region IList
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot { get; } = new object();
        void ICollection.CopyTo(Array array, int index) => snapshot.CopyTo((T[])array, index);
        object IList.this[int index]
        {
            get => snapshot[index];
            set => throw new NotSupportedException();
        }
        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => false;
        int IList.Add(object value) => throw new NotSupportedException();
        void IList.Clear() => throw new NotSupportedException();
        bool IList.Contains(object value) => snapshot.Contains((T)value);
        int IList.IndexOf(object value) => snapshot.IndexOf((T)value);
        void IList.Insert(int index, object value) => throw new NotSupportedException();
        void IList.Remove(object value) => throw new NotSupportedException();
        void IList.RemoveAt(int index) => throw new NotSupportedException();
        #endregion
    }
}
