using System;

namespace Sakuno.ING.Game.Models
{
    public partial class RepairingDock
    {
        private Ship _repairingShip;
        public Ship RepairingShip
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

        partial void UpdateCore(IRawRepairingDock raw, DateTimeOffset timeStamp)
        {
            RepairingShip = owner.AllShips[raw.RepairingShipId];
            UpdateTimer(timeStamp);
        }

        internal void UpdateTimer(DateTimeOffset timeStamp)
        {
            if (RepairingShip == null)
                TimeRemaining = null;
            else if (timeStamp > CompletionTime)
                TimeRemaining = default(TimeSpan);
            else
                TimeRemaining = CompletionTime - timeStamp;
        }
    }
}
