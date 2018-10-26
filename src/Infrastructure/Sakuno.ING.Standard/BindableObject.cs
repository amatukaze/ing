using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sakuno.ING
{
    public abstract class BindableObject : IBindable
    {
        private readonly List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)> handlers
            = new List<(SynchronizationContext, PropertyChangedEventHandler)>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (handlers)
                    handlers.Add((SynchronizationContext.Current, value));
            }
            remove
            {
                lock (handlers)
                    for (int i = 0; i < handlers.Count; i++)
                        if (handlers[i].handler == value)
                            handlers.RemoveAt(i--);
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
            => NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (isInBatchNotifyScope)
                batchNotifyArgs.Add(args);
            else
                lock (handlers)
                    foreach (var (syncContext, handler) in handlers)
                        syncContext.Post(o => handler(this, args), null);
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Set<T>(ref T field, T value, PropertyChangedEventArgs args)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(args);
            }
        }

        private bool isInBatchNotifyScope;
        private List<PropertyChangedEventArgs> batchNotifyArgs;

        protected BatchNotifyScope EnterBatchNotifyScope()
        {
            if (!isInBatchNotifyScope)
            {
                isInBatchNotifyScope = true;
                batchNotifyArgs = new List<PropertyChangedEventArgs>();
                return new BatchNotifyScope(this);
            }
            else
                return default;
        }

        private void BatchNotify()
        {
            var args = batchNotifyArgs;
            batchNotifyArgs = null;

            lock (handlers)
                foreach (var (syncContext, handler) in handlers)
                    syncContext.Post(o =>
                    {
                        foreach (var arg in args)
                            handler(this, arg);
                    }, null);

            isInBatchNotifyScope = false;
        }

        protected struct BatchNotifyScope : IDisposable
        {
            private readonly BindableObject owner;
            public BatchNotifyScope(BindableObject owner) => this.owner = owner;

            public void Dispose() => owner?.BatchNotify();
        }
    }
}
