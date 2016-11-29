using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticCustomTimeSpanGroupViewModel : SortieStatisticTimeSpanGroupViewModel
    {
        public DateTime MinDisplayDateStart { get; internal set; }

        DateTime r_SelectedDateStart;
        public DateTime SelectedDateStart
        {
            get { return r_SelectedDateStart; }
            set
            {
                if (r_SelectedDateStart != value)
                {
                    r_SelectedDateStart = value.AddDays(1.0).Date.AddSeconds(-1.0);
                    OnPropertyChanged(nameof(SelectedDateStart));

                    TimeSpanStart = new DateTimeOffset(r_SelectedDateStart).ToUnixTime().ToString();
                    Reload();
                }
            }
        }

        DateTime r_SelectedDateEnd;
        public DateTime SelectedDateEnd
        {
            get { return r_SelectedDateEnd; }
            set
            {
                if (r_SelectedDateEnd != value)
                {
                    r_SelectedDateEnd = value.AddDays(1.0).Date.AddSeconds(-1.0);
                    OnPropertyChanged(nameof(SelectedDateEnd));

                    TimeSpanEnd = new DateTimeOffset(r_SelectedDateEnd).ToUnixTime().ToString();
                    Reload();
                }
            }
        }

        public SortieStatisticCustomTimeSpanGroupViewModel() : base(SortieStatisticTimeSpanType.Custom)
        {
            var rNow = new DateTimeOffset(DateTime.Now.AddDays(1.0).Date.AddSeconds(-1.0));

            r_SelectedDateStart = r_SelectedDateEnd = rNow.DateTime;
            TimeSpanStart = TimeSpanEnd = rNow.ToUnixTime().ToString();
        }
    }
}
