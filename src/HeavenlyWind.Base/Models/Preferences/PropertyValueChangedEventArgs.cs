using System;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class PropertyValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }

        public PropertyValueChangedEventArgs(T rpOldValue, T rpNewValue)
        {
            OldValue = rpOldValue;
            NewValue = rpNewValue;
        }
    }
}
