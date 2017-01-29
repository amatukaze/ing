using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    class CustomTimeSpanViewModel : TimeSpanViewModel
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
                    r_SelectedDateStart = value.Date;

                    IsDateStartCalendarOpened = false;
                    TimeSpanStart = r_SelectedDateStart.AsOffset().ToUnixTime().ToString();

                    OnPropertyChanged(nameof(SelectedDateStart));
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
                    r_SelectedDateEnd = value.Date;

                    IsDateEndCalendarOpened = false;
                    TimeSpanEnd = r_SelectedDateEnd.AddDays(1.0).AsOffset().ToUnixTime().ToString();

                    OnPropertyChanged(nameof(SelectedDateEnd));
                }
            }
        }

        bool r_IsDateStartCalendarOpened;
        public bool IsDateStartCalendarOpened
        {
            get { return r_IsDateStartCalendarOpened; }
            set
            {
                if (r_IsDateStartCalendarOpened != value)
                {
                    r_IsDateStartCalendarOpened = value;
                    OnPropertyChanged(nameof(IsDateStartCalendarOpened));
                }
            }
        }

        bool r_IsDateEndCalendarOpened;
        public bool IsDateEndCalendarOpened
        {
            get { return r_IsDateEndCalendarOpened; }
            set
            {
                if (r_IsDateEndCalendarOpened != value)
                {
                    r_IsDateEndCalendarOpened = value;
                    OnPropertyChanged(nameof(IsDateEndCalendarOpened));
                }
            }
        }

        public CustomTimeSpanViewModel() : base(TimeSpanType.Custom)
        {
            SelectedDateStart = SelectedDateEnd = DateTime.Now;
        }
    }
}
