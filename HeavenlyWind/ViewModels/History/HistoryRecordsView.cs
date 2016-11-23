using Sakuno.KanColle.Amatsukaze.Internal;
using System.Linq;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class HistoryRecordsView<T> : ItemsView<T> where T : ModelBase
    {
        HistoryViewModelBase<T> r_Owner;

        List<T> r_Records;
        protected override IEnumerable<T> Source => r_Records;

        public T LastRecord => r_Records.Count > 0 ? r_Records.Last() : null;

        public HistoryRecordsView(HistoryViewModelBase<T> rpOwner)
        {
            r_Owner = rpOwner;
        }

        protected override void DisposeManagedResources()
        {
            r_Owner = null;
            r_Records?.Clear();

            base.DisposeManagedResources();
        }

        public void Load(List<T> rpRecords)
        {
            r_Records = rpRecords;

            Refresh();
        }

        public void Add(T rpRecord)
        {
            r_Records.Insert(0, rpRecord);

            Refresh();
        }

        protected override bool Filter(T rpItem) => r_Owner.Filter(rpItem);
    }
}
