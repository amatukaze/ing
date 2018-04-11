using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze
{
    public sealed class BindableCollection<T> : Collection<T>, IBindableCollection<T>, IList<T>
    {
#if WINDOWS_UWP
        private List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)>
            pHandlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();
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
        private void NotifyPropertyChanged(string propertyName = null)
        {
            var arg = new PropertyChangedEventArgs(propertyName);
            lock (pHandlers)
                foreach (var (syncContext, handler) in pHandlers)
                    syncContext.Post(o => handler(this, arg), null);
        }
#else
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
#endif

        private List<(SynchronizationContext syncContext, NotifyCollectionChangedEventHandler handler)>
            collectionHandlers = new List<(SynchronizationContext, NotifyCollectionChangedEventHandler)>();
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (collectionHandlers)
                    collectionHandlers.Add((SynchronizationContext.Current, value));
            }
            remove
            {
                lock (collectionHandlers)
                    for (int i = 0; i < collectionHandlers.Count; i++)
                        if (collectionHandlers[i].handler == value)
                            collectionHandlers.RemoveAt(i--);
            }
        }
        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            lock (collectionHandlers)
                foreach (var (context, handler) in collectionHandlers)
                    context.Post(o => handler(this, e), null);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            NotifyPropertyChanged(nameof(Count));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            base.RemoveItem(index);

            NotifyPropertyChanged(nameof(Count));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            NotifyPropertyChanged(nameof(Count));
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
