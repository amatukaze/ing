using Sakuno.KanColle.Amatsukaze.Models.Records;
using System;
using System.Data.SQLite;
using System.Globalization;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    class ResourceHistoryTypeKey : ModelBase
    {
        public ResourceHistoryType Type { get; }

        public Func<SQLiteDataReader, ResourceRecord> RecordConstructor { get; }

        public string Clause { get; }

        public long Maximum { get; private set; }

        public ResourceHistoryTypeKey(ResourceHistoryType rpType)
        {
            Type = rpType;

            switch (rpType)
            {
                case ResourceHistoryType.Detail:
                    RecordConstructor = r => new ResourceRecord(r);
                    break;

                case ResourceHistoryType.Daily:
                    RecordConstructor = r => new DailyResourceRecord(r);
                    Clause = "GROUP BY julianday(time, 'unixepoch', 'localtime', 'start of day', 'utc')";
                    break;

                case ResourceHistoryType.Weekly:
                    RecordConstructor = r => new WeeklyResourceRecord(r);
                    Clause = "GROUP BY julianday(time, 'unixepoch', 'localtime', 'weekday 0', '-6 days', 'start of day', 'utc')";
                    break;

                case ResourceHistoryType.Monthly:
                    RecordConstructor = r => new MonthlyResourceRecord(r);
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
                    Maximum = new DateTimeOffset(rNow.AddDays(-(rNow.DayOfWeek - DayOfWeek.Monday + 7) % 7).AddDays(7.0).Date).ToUnixTime();
                    break;

                case ResourceHistoryType.Monthly:
                    rNow = DateTime.Now;
                    Maximum = new DateTimeOffset(new DateTime(rNow.Year, rNow.Month, 1).AddMonths(1)).ToUnixTime();
                    break;
            }
        }

        class DailyResourceRecord : ResourceRecord
        {
            public override string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString("d");

            public DailyResourceRecord(SQLiteDataReader rpReader) : base(rpReader) { }
        }
        class WeeklyResourceRecord : ResourceRecord
        {
            static Calendar r_Calendar = CultureInfo.CurrentUICulture.Calendar;

            public override string Time
            {
                get
                {
                    var rMonday = DateTimeUtil.FromUnixTime(ID).LocalDateTime.Date;
                    rMonday = rMonday.AddDays(-(rMonday.DayOfWeek - DayOfWeek.Monday + 7) % 7);

                    var rMonth = rMonday.Month;

                    var rFirstMonday = new DateTime(rMonday.Year, rMonday.Month, 1);
                    rFirstMonday = rFirstMonday.AddDays(-(rFirstMonday.DayOfWeek - DayOfWeek.Monday + 7) % 7);

                    if (rFirstMonday.Month != rMonth)
                        rFirstMonday = rFirstMonday.AddDays(7.0);

                    var rCurrentWeek = r_Calendar.GetWeekOfYear(rMonday, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                    var rFirstMondayWeek = r_Calendar.GetWeekOfYear(rFirstMonday, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

                    var rWeek = rCurrentWeek - rFirstMondayWeek + 1;

                    return rMonday.ToString("Y") + " " + string.Format(StringResources.Instance.Main.ResourceHistory_Week_Format, rWeek);
                }
            }

            public WeeklyResourceRecord(SQLiteDataReader rpReader) : base(rpReader) { }
        }
        class MonthlyResourceRecord : ResourceRecord
        {
            public override string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString("Y");

            public MonthlyResourceRecord(SQLiteDataReader rpReader) : base(rpReader) { }
        }
    }
}
