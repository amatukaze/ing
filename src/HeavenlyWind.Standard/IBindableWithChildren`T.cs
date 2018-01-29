using System;

namespace Sakuno.KanColle.Amatsukaze
{
    internal interface IBindableWithChildren<out T> : IBindable where T : IBindable
    {
        IBindableCollection<T> Children { get; }
        event Action<IBindableCollection<T>> ChildrenChanged;
    }
}
