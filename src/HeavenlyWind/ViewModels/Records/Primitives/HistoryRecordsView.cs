using Sakuno.KanColle.Amatsukaze.Internal;
using System.Linq;
using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Models.Records;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    class HistoryRecordsView<T> : ItemsView<T> where T : ModelBase, IRecordID
    {
        HistoryViewModel<T> r_Owner;

        List<T> r_Records;
        protected override IEnumerable<T> Source => r_Records;

        public T LastRecord => r_Records.Count > 0 ? r_Records.Last() : null;

        public HistoryRecordsView(HistoryViewModel<T> rpOwner)
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
            r_Records.Add(rpRecord);

            Refresh();
        }

        public void Clear()
        {
            r_Records?.Clear();

            Refresh();
        }

        protected override bool Filter(T rpItem) => r_Owner.Filter(rpItem);
        protected override IEnumerable<T> Sort(IEnumerable<T> rpItems) => rpItems.OrderByDescending(r => r.ID);
    }
}
