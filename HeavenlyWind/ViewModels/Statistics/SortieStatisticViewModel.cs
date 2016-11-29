using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticViewModel : DisposableModelBase
    {
        public IList<SortieStatisticTimeSpanGroupViewModel> TimeSpans { get; private set; }

        SortieStatisticTimeSpanGroupViewModel r_SelectedTimeSpan;
        public SortieStatisticTimeSpanGroupViewModel SelectedTimeSpan
        {
            get { return r_SelectedTimeSpan; }
            set
            {
                if (r_SelectedTimeSpan != value)
                {
                    if (r_SelectedTimeSpan != null)
                        r_SelectedTimeSpan.Maps = null;

                    r_SelectedTimeSpan = value;
                    OnPropertyChanged(nameof(SelectedTimeSpan));

                    if (r_SelectedTimeSpan != null)
                        Load();
                }
            }
        }

        public SortieStatisticViewModel()
        {
            IEnumerable<SortieStatisticTimeSpanGroupViewModel> rTimeSpans = Enumerable.Range(0, 6).Select(r => new SortieStatisticDefaultTimeSpanGroupViewModel((SortieStatisticTimeSpanType)r));

            TimeSpans = rTimeSpans.ToArray();
            r_SelectedTimeSpan = TimeSpans[0];
        }

        public void Load() => Task.Run((Action)r_SelectedTimeSpan.Reload);

        protected override void DisposeManagedResources()
        {
            SelectedTimeSpan = null;
            TimeSpans = null;
        }
    }
}
