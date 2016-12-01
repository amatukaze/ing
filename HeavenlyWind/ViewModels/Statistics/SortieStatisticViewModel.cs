using Sakuno.KanColle.Amatsukaze.Game;
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

        public IList<SortieStatisticAreaViewModel> Areas { get; }

        internal string PastAreas { get; }

        public SortieStatisticViewModel()
        {
            var rTimeSpans = Enumerable.Range(0, 6).Select(r => new SortieStatisticDefaultTimeSpanGroupViewModel(this, (SortieStatisticTimeSpanType)r)).ToList<SortieStatisticTimeSpanGroupViewModel>();
            rTimeSpans.Add(CustomTimeSpan = new SortieStatisticCustomTimeSpanGroupViewModel(this));
            TimeSpans = rTimeSpans.ToArray();
            r_SelectedTimeSpan = TimeSpans[0];

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT group_concat(DISTINCT map / 10) FROM sortie WHERE id >= strftime('%s', 'now', 'localtime', 'start of month', '-1 month', 'utc') AND map NOT IN (" + string.Join(", ", KanColleGame.Current.MasterInfo.Maps.Keys) + ");";
                var rPastAreas = rCommand.ExecuteScalar();
                if (rPastAreas != DBNull.Value)
                    PastAreas = (string)rPastAreas;

                var rAreas = KanColleGame.Current.MasterInfo.MapAreas.Values.Select(r => new SortieStatisticAreaViewModel(r) { IsSelectedChangedCallback = Load }).ToList();
                if (PastAreas != null)
                    rAreas.Add(new SortieStatisticAreaViewModel(null) { IsSelectedChangedCallback = Load });
                Areas = rAreas.ToArray();

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
