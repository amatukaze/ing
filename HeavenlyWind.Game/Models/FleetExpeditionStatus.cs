using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetExpeditionStatus : CountdownModelBase
    {
        Fleet r_Fleet;

        ExpeditionInfo r_Expedition;
        public ExpeditionInfo Expedition
        {
            get { return r_Expedition; }
            private set
            {
                if (r_Expedition != value)
                {
                    r_Expedition = value;
                    OnPropertyChanged(nameof(Expedition));
                }
            }
        }

        public override TimeSpan RemainingTimeToNotify => TimeSpan.FromMinutes(1.0);

        public event Action<string, string> Returned = delegate { };

        internal FleetExpeditionStatus(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update(long[] rpRawData)
        {
            var rState = (int)rpRawData[0];
            var rExpeditionID = (int)rpRawData[1];
            var rTimeToComplete = rpRawData[2];

            if (rState != 0)
            {
                if (Expedition == null)
                    foreach (var rShip in r_Fleet.Ships)
                        rShip.State |= ShipState.Expedition;

                Expedition = KanColleGame.Current.MasterInfo.Expeditions[rExpeditionID];
                TimeToComplete = new DateTimeOffset?(DateTimeUtil.UnixEpoch.AddMilliseconds(rTimeToComplete));
            }
            else
            {
                if (Expedition != null)
                    foreach (var rShip in r_Fleet.Ships)
                        rShip.State &= ~ShipState.Expedition;

                Expedition = null;
                TimeToComplete = null;
            }
        }

        protected override void TimeOut() => Returned(r_Fleet.Name, r_Expedition?.TranslatedName);
    }
}
