using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticTimeSpanGroupViewModel : ModelBase
    {
        public SortieStatisticTimeSpanType Type { get; }

        public IList<ModelBase> Maps { get; private set; }

        public SortieStatisticTimeSpanGroupViewModel(SortieStatisticTimeSpanType rpType)
        {
            Type = rpType;
        }

        public void Update(IEnumerable<SortieStatisticData> rpItems)
        {
            Maps = rpItems.Select(r => new SortieStatisticMapItem(r)).ToList<ModelBase>();
            Maps.Insert(0, new SortieStatisticTotalItem(rpItems));
            OnPropertyChanged(string.Empty);
        }
    }
}
