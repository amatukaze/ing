namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    class GeneralTimeSpanViewModel : TimeSpanViewModel
    {
        public GeneralTimeSpanViewModel(TimeSpanType rpType) : base(rpType)
        {
            switch (rpType)
            {
                case TimeSpanType.All:
                    TimeSpanStart = "0";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case TimeSpanType.Recent24Hours:
                    TimeSpanStart = "strftime('%s', 'now', '-1 days')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case TimeSpanType.Recent3Days:
                    TimeSpanStart = "strftime('%s', 'now', '-3 days')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;

                case TimeSpanType.Recent7Days:
                    TimeSpanStart = "strftime('%s', 'now', '-7 days')";
                    TimeSpanEnd = "strftime('%s', 'now')";
                    break;
            }
        }
    }
}
