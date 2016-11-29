using Sakuno.KanColle.Amatsukaze.Models.Statistics;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticDefaultTimeSpanGroupViewModel : SortieStatisticTimeSpanGroupViewModel
    {
        public SortieStatisticDefaultTimeSpanGroupViewModel(SortieStatisticTimeSpanType rpType) : base(rpType)
        {
            switch (rpType)
            {
                case SortieStatisticTimeSpanType.Today:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of day', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.ThisWeek:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.ThisMonth:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of month', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.Yesterday:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of day', '-1 days', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'start of day', 'utc')";
                    break;

                case SortieStatisticTimeSpanType.LastWeek:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'weekday 0', '-13 days', 'start of day', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')";
                    break;

                case SortieStatisticTimeSpanType.LastMonth:
                    TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of month', '-1 month', 'utc')";
                    TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'start of month', 'utc')";
                    break;
            }
        }
    }
}
