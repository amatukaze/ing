using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class FilterKeyCollection<T> : IList, IEnumerable<T>, INotifyCollectionChanged where T : class
    {
        static NotifyCollectionChangedEventArgs r_ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        T r_AllKey;

        List<T> r_Keys = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public FilterKeyCollection(T rpAllKey)
        {
            r_AllKey = rpAllKey;

            r_Keys.Add(r_AllKey);
        }

        public void Add(T rpKey)
        {
            r_Keys.Add(rpKey);

            OnCollectionReset();
        }
        public void AddRange(IEnumerable<T> rpKeys)
        {
            r_Keys.AddRange(rpKeys);

            OnCollectionReset();
        }

        public void AddIfAbsent(T rpKey)
        {
            if (rpKey == r_AllKey)
                return;

            foreach (var rKey in r_Keys)
                if (rKey == rpKey)
                    return;

            r_Keys.Add(rpKey);
        }

        public IEnumerator<T> GetEnumerator() => r_Keys.GetEnumerator();

        void OnCollectionReset()
        {
            DispatcherUtil.UIDispatcher.BeginInvoke(new Action(() => CollectionChanged?.Invoke(this, r_ResetEventArgs)));
        }

        #region

        int ICollection.Count => r_Keys.Count;

        bool IList.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        bool IList.IsFixedSize => false;
        object ICollection.SyncRoot { get { throw new NotSupportedException(); } }
        object IList.this[int rpIndex]
        {
            get { return r_Keys[rpIndex]; }
            set { throw new NotSupportedException(); }
        }
        int IList.Add(object rpValue) { throw new NotSupportedException(); }
        bool IList.Contains(object rpValue) { throw new NotSupportedException(); }
        void IList.Clear() { throw new NotSupportedException(); }
        int IList.IndexOf(object rpValue) => r_Keys.IndexOf((T)rpValue);
        void IList.Insert(int rpIndex, object rpValue) { throw new NotSupportedException(); }
        void IList.Remove(object rpValue) { throw new NotSupportedException(); }
        void IList.RemoveAt(int rpIndex) { throw new NotSupportedException(); }
        void ICollection.CopyTo(Array rpArray, int rpIndex) { throw new NotSupportedException(); }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
