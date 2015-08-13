using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.Collections
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        static PropertyChangedEventArgs r_CountProperty = new PropertyChangedEventArgs("Count");
        static PropertyChangedEventArgs r_ItemsProperty = new PropertyChangedEventArgs("Item[]");

        List<T> r_List;

        public ObservableRangeCollection()
            : base() { r_List = (List<T>)Items; }
        public ObservableRangeCollection(IEnumerable<T> rpCollection)
            : base(rpCollection) { r_List = (List<T>)Items; }
        public ObservableRangeCollection(List<T> rpCollection)
            : base(rpCollection) { r_List = (List<T>)Items; }

        public void AddRange(IEnumerable<T> rpCollection)
        {
            if (rpCollection == null)
                throw null;

            CheckReentrancy();
            r_List.AddRange(rpCollection);

            OnPropertyChanged(r_CountProperty);
            OnPropertyChanged(r_ItemsProperty);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}
