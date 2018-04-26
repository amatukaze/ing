using System;

namespace Sakuno.ING.Settings
{
    public interface ISettingItem<T> : IBindable
    {
        T Value { get; set; }
        event Action<T> ValueChanged;
    }
}
