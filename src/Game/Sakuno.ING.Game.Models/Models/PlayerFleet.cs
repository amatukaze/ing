using System.Collections.ObjectModel;

namespace Sakuno.ING.Game.Models
{
    public partial class PlayerFleet
    {
        private ObservableCollection<PlayerShip> _ships = new ObservableCollection<PlayerShip>();
        public ReadOnlyObservableCollection<PlayerShip> Ships { get; private set; }

        partial void CreateCore()
        {
            _ships = new ObservableCollection<PlayerShip>();
            Ships = new ReadOnlyObservableCollection<PlayerShip>(_ships);
        }

        partial void UpdateCore(RawFleet raw)
        {
            for (var i = 0; i < _ships.Count || i < raw.ShipIds.Length; i++)
                if (i >= raw.ShipIds.Length)
                {
                    _ships.RemoveAt(i);
                    i--;
                }
                else if (i >= _ships.Count)
                    _ships.Add(_owner.Ships[raw.ShipIds[i]]);
                else if (raw.ShipIds[i] != _ships[i].Id)
                    _ships[i] = _owner.Ships[raw.ShipIds[i]];

            ExpeditionState = raw.ExpeditionStatus.State;
            Expedition = _owner.MasterData.Expeditions[raw.ExpeditionStatus.ExpeditionId];
        }
    }
}
