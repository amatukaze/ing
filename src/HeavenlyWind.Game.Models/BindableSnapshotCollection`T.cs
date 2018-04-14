using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public partial class BindableSnapshotCollection<T> : IDisposable, IBindableCollection<T>
        where T : class
    {
        private readonly IUpdationSource source;
        private T[] snapshot;

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

        public BindableSnapshotCollection(IUpdationSource source, IEnumerable<T> query)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            _query = query ?? throw new ArgumentNullException(nameof(query));

            if (BindableObject.ThreadSafeEnabled)
                pHandlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();

            snapshot = query.ToArray();
            source.Updated += Refresh;
        }

        public void Dispose() => source.Updated -= Refresh;

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
        private void NotifyCollectionChanged(IEnumerable<NotifyCollectionChangedEventArgs> args)
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
            var @new = Query.ToArray();
            var diff = SequenceDiffer(snapshot, @new);
            snapshot = @new;
            if (diff.Length == 0) return;

            var args = new NotifyCollectionChangedEventArgs[diff.Length];
            int offset = 0;
            for (int i = 0; i < args.Length; i++)
            {
                var d = diff[i];
                if (d.IsAdd)
                    args[i] = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, d.Item, d.OriginalIndex + (offset++));
                else
                    args[i] = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, d.Item, d.OriginalIndex + (offset--));
            }
            NotifyCollectionChanged(args);
        }

        public int Count => snapshot.Length;
        public T this[int index] => snapshot[index];

        #region IEnumerable
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public struct Enumerator : IEnumerator<T>
        {
            private T[] array;
            private int index;
            public Enumerator(BindableSnapshotCollection<T> origin)
            {
                array = origin.snapshot;
                index = 0;
            }

            public T Current => array[index];
            object IEnumerator.Current => array[index];

            public void Dispose() { }
            public bool MoveNext() => ++index < array.Length;
            public void Reset() => index = 0;
        }
        #endregion

        #region IList
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot { get; } = new object();
        void ICollection.CopyTo(Array array, int index) => snapshot.CopyTo(array, index);
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
        int IList.IndexOf(object value)
        {
            T v = (T)value;
            for (int i = 0; i < snapshot.Length; i++)
                if (snapshot[i] == v) return i;
            return -1;
        }
        void IList.Insert(int index, object value) => throw new NotSupportedException();
        void IList.Remove(object value) => throw new NotSupportedException();
        void IList.RemoveAt(int index) => throw new NotSupportedException();
        #endregion
    }
}
