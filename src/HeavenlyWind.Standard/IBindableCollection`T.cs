using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sakuno.KanColle.Amatsukaze
{
    public interface IBindableCollection<out T> : IReadOnlyList<T>, IList, IBindable, INotifyCollectionChanged
    {
    }
}
