using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetExpeditionStatus : CountdownModelBase
    {
        Fleet r_Fleet;

        ExpeditionInfo r_Current;
        public ExpeditionInfo Current
        {
            get { return r_Current; }
            private set
            {
                if (r_Current != value)
                {
                    r_Current = value;
                    OnPropertyChanged(nameof(Current));
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
            var rCompleteTime = rpRawData[2];

            if (rState != 0)
            {
                Current = KanColleGame.Current.MasterInfo.Expeditions[rExpeditionID];
                CompleteTime = new DateTimeOffset?(DateTimeUtil.UnixEpoch.AddMilliseconds(rCompleteTime));
            }
            else
            {
                Current = null;
                CompleteTime = null;
            }
        }

        protected override void TimeOut() => Returned(r_Fleet.Name, Current?.Name);
    }
}
