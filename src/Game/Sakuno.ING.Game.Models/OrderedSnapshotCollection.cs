using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Sakuno.ING.Game
{
    internal class OrderedSnapshotCollection<T> : IBindableCollection<T>
        where T : IComparable<T>
    {
        private List<T> snapshot;
        private readonly IEnumerable<T> query;
        public OrderedSnapshotCollection(IEnumerable<T> source, Func<T, bool> filter)
        {
            query = source.Where(filter);
            snapshot = query.ToList();
            if (source is IUpdationSource updation)
                updation.Updated += Update;
        }

        public void Update()
        {
            var @new = query.ToList();
            var args = new List<NotifyCollectionChangedEventArgs>();

            int i = 0, j = 0, offset = 0;
            while (i < snapshot.Count || j < @new.Count)
                if (i == snapshot.Count)
                {
                    args.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, @new[j], i + offset++));
                    j++;
                }
                else if (j == @new.Count)
                {
                    args.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, snapshot[i], i + offset--));
                    i++;
                }
                else
                {
                    int compare = snapshot[i].CompareTo(@new[j]);
                    if (compare > 0)
                    {
                        args.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, @new[j], i + offset++));
                        j++;
                    }
                    else if (compare < 0)
                    {
                        args.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, snapshot[i], i + offset--));
                        i++;
                    }
                    else
                    {
                        i++;
                        j++;
                    }
                }

            snapshot = @new;
            if (args.Count > 0)
            {
                NotifyPropertyChanged(nameof(Count));
                NotifyCollectionChanged(args.ToArray());
            }
        }

        #region PropertyChange
        private readonly List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)> pHandlers
            = new List<(SynchronizationContext, PropertyChangedEventHandler)>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (pHandlers)
                    pHandlers.Add((SynchronizationContext.Current, value));
            }
            remove
            {
                lock (pHandlers)
                    for (int i = 0; i < pHandlers.Count; i++)
                        if (pHandlers[i].handler == value)
                            pHandlers.RemoveAt(i--);
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var arg = new PropertyChangedEventArgs(propertyName);
            lock (pHandlers)
                foreach (var (syncContext, handler) in pHandlers)
                    syncContext.Post(o => handler(this, arg), null);
        }
        #endregion

        #region CollectionChange
        private readonly List<(SynchronizationContext syncContext, NotifyCollectionChangedEventHandler handler)>
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
