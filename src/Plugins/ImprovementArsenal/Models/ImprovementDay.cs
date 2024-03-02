using System;
using System.Globalization;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementDay : ModelBase
    {
        internal static readonly ImprovementDay[] Yes;
        internal static readonly ImprovementDay[] No;

        public DayOfWeek Day { get; }

        string r_DayName;
        public string DayName
        {
            get { return r_DayName; }
            private set
            {
                if (r_DayName != value)
                {
                    r_DayName = value;
                    OnPropertyChanged(nameof(DayName));
                }
            }
        }

        public bool IsToday { get; private set; }

        public bool Improvable { get; }

        static ImprovementDay()
        {
            var rDays = (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek));

            Yes = rDays.Select(r => new ImprovementDay(r, true)).ToArray();
            No = rDays.Select(r => new ImprovementDay(r, false)).ToArray();

            Preference.Instance.Language.Subscribe(r =>
            {
                CultureInfo rCultureInfo;

                var rInfo = StringResources.Instance.InstalledLanguages.SingleOrDefault(rpInfo => rpInfo.Directory == r);
                if (rInfo == null)
                    rCultureInfo = CultureInfo.CurrentUICulture;
                else
                    rCultureInfo = CultureInfo.GetCultureInfo(rInfo.CultureName);

                foreach (var rDay in Yes.Concat(No))
                    rDay.DayName = rCultureInfo.DateTimeFormat.GetShortestDayName(rDay.Day);
            }, true);
        }
        ImprovementDay(DayOfWeek rpDay, bool rpImprovable)
        {
            Day = rpDay;

            Improvable = rpImprovable;
        }

        public static ImprovementDay Get(int rpDayOfWeek, bool rpImprovable)
        {
            var rArray = rpImprovable ? Yes : No;
            return rArray[rpDayOfWeek];
        }

        public static void UpdateToday(DayOfWeek rpToday)
        {
            foreach (var rDay in Yes.Concat(No))
                rDay.UpdateTodayCore(rpToday);
        }
        void UpdateTodayCore(DayOfWeek rpToday)
        {
            IsToday = Day == rpToday;
            OnPropertyChanged(nameof(IsToday));
        }
    }
}
