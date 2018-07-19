using System;

namespace Sakuno.ING.Settings
{
    public interface ISettingItem<T> : IBindable
    {
        T Value { get; set; }
        T InitialValue { get; }
        event Action<T> ValueChanged;
    }
}
