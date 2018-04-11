using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze
{
    public abstract class BindableObject : IBindable
    {
#if WINDOWS_UWP
        private List<(SynchronizationContext syncContext, PropertyChangedEventHandler handler)>
            handlers = new List<(SynchronizationContext, PropertyChangedEventHandler)>();
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
        {
            var arg = new PropertyChangedEventArgs(propertyName);
            lock (handlers)
                foreach (var (syncContext, handler) in handlers)
                    syncContext.Post(o => handler(this, arg), null);
        }
#else
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
#endif

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
