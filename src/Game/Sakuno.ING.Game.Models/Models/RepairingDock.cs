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
            Timer.Init(raw.CompletionTime, timeStamp);
        }

        internal void Instant()
        {
            State = RepairingDockState.Empty;
            RepairingShip = null;
            Timer.Clear();
        }

        internal bool UpdateTimer(DateTimeOffset timeStamp) => Timer.Update(timeStamp);

        public CountDown Timer { get; } = new CountDown();
    }
}
