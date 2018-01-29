using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.KanColle.Amatsukaze
{
    public interface IBindableCollection<out T> : IReadOnlyList<T>, IList, IBindable, INotifyCollectionChanged
    {
        event PropertyChangedEventHandler ChildrenPropertyChanged;
    }
}
