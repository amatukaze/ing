namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class RepairingDock
    {
        private Ship _repairingShip;
        public Ship RepairingShip
        {
            get => _repairingShip;
            internal set
            {
                if (_repairingShip != value)
                {
                    _repairingShip.IsRepairing = false;
                    value.IsRepairing = true;
                    _repairingShip = value;
                    NotifyPropertyChanged();
                }
            }
        }

        partial void UpdateCore(IRawRepairingDock raw)
        {
            RepairingShip = shipTable.TryGetOrDummy(raw.RepairingShipId);
        }
    }
}
