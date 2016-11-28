using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    abstract class ItemsView<T> : DisposableObject, IList, IList<T>, INotifyCollectionChanged
    {
        static NotifyCollectionChangedEventArgs r_ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        protected abstract IEnumerable<T> Source { get; }

        List<T> r_View = new List<T>(100);
        public IList<T> View { get; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Refresh()
        {
            if (Source == null)
                return;

            BeforeRefresh();

            var rItems = Source.Where(Filter);

            rItems = Sort(rItems);

            r_View.Clear();
            r_View.AddRange(rItems);

            OnCollectionReset();
        }

        protected virtual void BeforeRefresh() { }

        public IEnumerator<T> GetEnumerator() => r_View.GetEnumerator();

        protected abstract bool Filter(T rpItem);
        protected virtual IEnumerable<T> Sort(IEnumerable<T> rpItems) => rpItems;

        protected override void DisposeManagedResources()
        {
            r_View.Clear();

            OnCollectionReset();
        }

        protected void OnCollectionReset()
        {
            DispatcherUtil.UIDispatcher.InvokeAsync(() => CollectionChanged?.Invoke(this, r_ResetEventArgs));
        }

        #region

        public int Count => r_View.Count;

        bool IList.IsReadOnly => false;
        bool ICollection<T>.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        bool IList.IsFixedSize => false;
        object ICollection.SyncRoot { get { throw new NotSupportedException(); } }
        object IList.this[int rpIndex]
        {
            get { return r_View[rpIndex]; }
            set { throw new NotSupportedException(); }
        }
        T IList<T>.this[int rpIndex]
        {
            get { return r_View[rpIndex]; }
            set { throw new NotSupportedException(); }
        }
        int IList.Add(object rpValue) { throw new NotSupportedException(); }
        bool IList.Contains(object rpValue) => r_View.Contains((T)rpValue);
        void IList.Clear() { throw new NotSupportedException(); }
        int IList.IndexOf(object rpValue) => r_View.IndexOf((T)rpValue);
        void IList.Insert(int rpIndex, object rpValue) { throw new NotSupportedException(); }
        void IList.Remove(object rpValue) { throw new NotSupportedException(); }
        void IList.RemoveAt(int rpIndex) { throw new NotSupportedException(); }
        void ICollection.CopyTo(Array rpArray, int rpIndex) { throw new NotSupportedException(); }
        void ICollection<T>.Add(T rpValue) { throw new NotSupportedException(); }
        bool ICollection<T>.Contains(T rpValue) => r_View.Contains(rpValue);
        void ICollection<T>.Clear() { throw new NotSupportedException(); }
        int IList<T>.IndexOf(T rpValue) => r_View.IndexOf(rpValue);
        void IList<T>.Insert(int rpIndex, T rpValue) { throw new NotSupportedException(); }
        bool ICollection<T>.Remove(T rpValue) { throw new NotSupportedException(); }
        void IList<T>.RemoveAt(int rpIndex) { throw new NotSupportedException(); }
        void ICollection<T>.CopyTo(T[] rpArray, int rpIndex) { throw new NotSupportedException(); }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
