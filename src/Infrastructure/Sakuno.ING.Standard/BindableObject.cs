using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sakuno.ING
{
    public abstract class BindableObject : IBindable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null) =>
            NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args) =>
            PropertyChanged?.Invoke(this, args);

        protected void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            NotifyPropertyChanged(propertyName);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Set<T>(ref T field, T value, PropertyChangedEventArgs args)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            NotifyPropertyChanged(args);
        }
    }
}
