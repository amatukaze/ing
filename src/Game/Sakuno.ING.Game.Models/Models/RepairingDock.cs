using System;

namespace Sakuno.ING.Game.Models
{
    public partial class RepairingDock
    {
        private HomeportShip _repairingShip;
        public HomeportShip RepairingShip
        {
            get => _repairingShip;
            internal set
            {
                if (_repairingShip != value)
                {
                    _repairingShip?.SetRepaired();
                    if (value != null)
                        value.IsRepairing = true;
                    _repairingShip = value;
                    NotifyPropertyChanged();
                }
            }
        }

        partial void UpdateCore(RawRepairingDock raw, DateTimeOffset timeStamp)
        {
            RepairingShip = owner.AllShips[raw.RepairingShipId];
            if (Timer.SetCompletionTime(raw.CompletionTime, timeStamp))
                owner.Notification.SetRepairCompletion(this, raw.CompletionTime);
        }

        internal void Instant()
        {
            State = RepairingDockState.Empty;
            RepairingShip = null;
            Timer.Clear();
            owner.Notification.SetRepairCompletion(this, null);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp) => Timer.Update(timeStamp);

        public CountDown Timer { get; } = new CountDown();
    }
}
