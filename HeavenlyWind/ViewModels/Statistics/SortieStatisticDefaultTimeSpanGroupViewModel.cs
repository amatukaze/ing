using Sakuno.KanColle.Amatsukaze.Models.Statistics;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticDefaultTimeSpanGroupViewModel : SortieStatisticTimeSpanGroupViewModel
    {
        public SortieStatisticDefaultTimeSpanGroupViewModel(SortieStatisticViewModel rpOwner, SortieStatisticTimeSpanType rpType) : base(rpOwner, rpType)
        {
            var r8amJstOrigin = Preference.Instance.Game.SortieStatistic_8amJstOrigin.Value;

            switch (rpType)
            {
                case SortieStatisticTimeSpanType.Today:
                    TimeSpanStart = !r8amJstOrigin ? "strftime('%s', 'now', 'localtime', 'start of day', 'utc')"
                                                   : "strftime('%s', 'now', '+1 hour', 'start of day', '-1 hour')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.ThisWeek:
                    TimeSpanStart = !r8amJstOrigin ? "strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')"
                                                   : "strftime('%s', 'now', '+1 hour', 'weekday 0', '-6 days', 'start of day', '-1 hour')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.ThisMonth:
                    TimeSpanStart = !r8amJstOrigin ? "strftime('%s', 'now', 'localtime', 'start of month', 'utc')"
                                                   : "strftime('%s', 'now', '+1 hour', 'start of month', '-1 hour')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case SortieStatisticTimeSpanType.Yesterday:
                    if (!r8amJstOrigin)
                    {
                        TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of day', '-1 days', 'utc')";
                        TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'start of day', 'utc')";
                    }
                    else
                    {
                        TimeSpanStart = "strftime('%s', 'now', '+1 hour', 'start of day', '-25 hour')";
                        TimeSpanEnd = "strftime('%s', 'now', '+1 hour', 'start of day', '-1 hour')";
                    }
                    break;

                case SortieStatisticTimeSpanType.LastWeek:
                    if (!r8amJstOrigin)
                    {
                        TimeSpanStart = "strftime('%s', 'now', 'localtime', 'weekday 0', '-13 days', 'start of day', 'utc')";
                        TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')";
                    }
                    else
                    {
                        TimeSpanStart = "strftime('%s', 'now', '+1 hour', 'weekday 0', '-13 days', 'start of day', '-1 hour')";
                        TimeSpanEnd = "strftime('%s', 'now', '+1 hour', 'weekday 0', '-13 days', 'start of day', '-1 hour')";
                    }
                    break;

                case SortieStatisticTimeSpanType.LastMonth:
                    if (!r8amJstOrigin)
                    {
                        TimeSpanStart = "strftime('%s', 'now', 'localtime', 'start of month', '-1 month', 'utc')";
                        TimeSpanEnd = "strftime('%s', 'now', 'localtime', 'start of month', 'utc')";
                    }
                    else
                    {
                        TimeSpanStart = "strftime('%s', 'now', '+1 hour', 'start of month', '-1 month', '-1 hour')";
                        TimeSpanEnd = "strftime('%s', 'now', '+1 hour', 'start of month', '-1 hour')";
                    }
                    break;
            }
        }
    }
}
