using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Sakuno.ING
{
    public sealed class BindableCollection<T> : Collection<T>, IBindableCollection<T>, IList<T>
    {
        #region PropertyChange
        private List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)> pHandlers;
        private PropertyChangedEventHandler pHandler;

        public BindableCollection()
        {
            if (BindableObject.ThreadSafeEnabled)
                pHandlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();
        }

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
        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            lock (cHandlers)
                foreach (var (context, handler) in cHandlers)
                    context.Post(o => handler(this, e), null);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);

            ItemRemoved?.Invoke(oldItem);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
            ItemAdded?.Invoke(item);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            NotifyPropertyChanged(nameof(Count));
            ItemAdded?.Invoke(item);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            base.RemoveItem(index);

            NotifyPropertyChanged(nameof(Count));
            ItemRemoved?.Invoke(oldItem);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
                ItemRemoved?.Invoke(item);
            base.ClearItems();

            NotifyPropertyChanged(nameof(Count));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Exchange(int index1, int index2)
        {
            var item1 = this[index1];
            var item2 = this[index2];
            base.SetItem(index1, item2);
            base.SetItem(index2, item1);

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item1, index1));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item2, index1));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item2, index2));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item1, index2));
        }

        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;
    }
}
