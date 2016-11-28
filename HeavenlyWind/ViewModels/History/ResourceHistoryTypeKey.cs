using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ResourceHistoryTypeKey : ModelBase
    {
        public ResourceHistoryType Type { get; }

        public string Clause { get; }

        public long Maximum { get; private set; }

        public ResourceHistoryTypeKey(ResourceHistoryType rpType)
        {
            Type = rpType;

            switch (rpType)
            {
                case ResourceHistoryType.Daily:
                    Clause = "GROUP BY julianday(time, 'unixepoch', 'localtime', 'start of day', 'utc')";
                    break;

                case ResourceHistoryType.Weekly:
                    Clause = "GROUP BY julianday(time, 'unixepoch', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')";
                    break;

                case ResourceHistoryType.Monthly:
                    Clause = "GROUP BY julianday(time, 'unixepoch', 'localtime', 'start of month', 'utc')";
                    break;
            }
        }

        public void Update()
        {
            DateTime rNow;

            switch (Type)
            {
                case ResourceHistoryType.Daily:
                    Maximum = new DateTimeOffset(DateTime.Now.AddDays(1.0).Date).ToUnixTime();
                    break;

                case ResourceHistoryType.Weekly:
                    rNow = DateTime.Now;
                    Maximum = new DateTimeOffset(rNow.AddDays((1 - (int)rNow.DayOfWeek + 7) % 7).AddDays(7.0).Date).ToUnixTime();
                    break;

                case ResourceHistoryType.Monthly:
                    rNow = DateTime.Now;
                    Maximum = new DateTimeOffset(new DateTime(rNow.Year, rNow.Month, 1).AddMonths(1)).ToUnixTime();
                    break;
            }
        }
    }
}
