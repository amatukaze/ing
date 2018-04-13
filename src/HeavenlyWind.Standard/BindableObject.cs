using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze
{
    public abstract class BindableObject : IBindable
    {
        public static bool ThreadSafeEnabled { get; set; }

        private List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)> handlers;
        private PropertyChangedEventHandler handler;

        protected BindableObject()
        {
            if (ThreadSafeEnabled)
                handlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (ThreadSafeEnabled)
                    lock (handlers)
                        handlers.Add((SynchronizationContext.Current, value));
                else
                    handler += value;
            }
            remove
            {
                if (ThreadSafeEnabled)
                {
                    lock (handlers)
                        for (int i = 0; i < handlers.Count; i++)
                            if (handlers[i].handler == value)
                                handlers.RemoveAt(i--);
                }
                else
                    handler -= value;
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var arg = new PropertyChangedEventArgs(propertyName);
            if (ThreadSafeEnabled)
                lock (handlers)
                    foreach (var (syncContext, handler) in handlers)
                        syncContext.Post(o => handler(this, arg), null);
            else
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
