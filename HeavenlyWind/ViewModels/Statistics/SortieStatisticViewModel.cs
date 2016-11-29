using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticViewModel : DisposableModelBase
    {
        public IList<SortieStatisticTimeSpanGroupViewModel> TimeSpans { get; private set; }

        public SortieStatisticCustomTimeSpanGroupViewModel CustomTimeSpan { get; }

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

                    r_SelectedTimeSpan?.Reload();
                }
            }
        }

        public SortieStatisticViewModel()
        {
            IEnumerable<SortieStatisticTimeSpanGroupViewModel> rTimeSpans = Enumerable.Range(0, 6).Select(r => new SortieStatisticDefaultTimeSpanGroupViewModel((SortieStatisticTimeSpanType)r));

            CustomTimeSpan = new SortieStatisticCustomTimeSpanGroupViewModel();

            TimeSpans = rTimeSpans.Concat(new[] { CustomTimeSpan }).ToArray();
            r_SelectedTimeSpan = TimeSpans[0];

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT min(id) FROM sortie_consumption;";
                CustomTimeSpan.MinDisplayDateStart = DateTimeUtil.FromUnixTime((long)rCommand.ExecuteScalar()).ToOffset(DateTimeOffset.Now.Offset).DateTime.Date;
            }
        }

        public void Load() => r_SelectedTimeSpan.Reload();

        protected override void DisposeManagedResources()
        {
            SelectedTimeSpan = null;
            TimeSpans = null;
        }
    }
}
