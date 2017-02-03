using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipAnchorageRepairStatus : CountdownModelBase
    {
        Ship r_Ship;

        public string TimeToCompleteString => TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTimeString
        {
            get
            {
                if (!RemainingTime.HasValue)
                    return string.Empty;

                var rRemainingTime = RemainingTime.Value;
                if (rRemainingTime.TotalHours < 1.0)
                    return rRemainingTime.ToString(@"mm\:ss");
                else
                    return rRemainingTime.ToString(@"hh\:mm\:ss");
            }
        }

        internal ShipAnchorageRepairStatus(Ship rpShip)
        {
            r_Ship = rpShip;
        }

        internal void Update()
        {
            var rRepairTime = r_Ship.RepairTime.Value;
            if (rRepairTime.TotalMinutes <= 20.0)
                rRepairTime = TimeSpan.FromMinutes(20.0);
            else
            {
                var rDamage = r_Ship.HP.Maximum - r_Ship.HP.Current;
                var rMinutePerHP = rRepairTime.TotalMinutes / rDamage;
                if (rMinutePerHP > 20.0)
                    rRepairTime = TimeSpan.FromMinutes(rDamage * 20.0);
            }

            TimeToComplete = DateTimeOffset.Now + rRepairTime;
        }

        internal void Offset(TimeSpan rpTimeSpan) => TimeToComplete -= rpTimeSpan;

        protected override void OnTick()
        {
            OnPropertyChanged(nameof(TimeToCompleteString));
            OnPropertyChanged(nameof(RemainingTimeString));
        }

        protected override void TimeOut() { }
    }
}
