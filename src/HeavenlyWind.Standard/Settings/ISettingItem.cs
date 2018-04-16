using System;

namespace Sakuno.KanColle.Amatsukaze.Settings
{
    public interface ISettingItem<T> : IBindable
    {
        T Value { get; set; }
        event Action<T> ValueChanged;
    }
}
