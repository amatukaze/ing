using System.Collections.ObjectModel;
using System.Linq;

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
            ExpeditionCompletionTime = raw.ExpeditionStatus.ReturnTime;
        }

        internal void ChangeComposition(int? index, PlayerShip? ship)
        {
            if (index is not int i)
            {
                while (_ships.Count > 1)
                    _ships.RemoveAt(1);
                return;
            }

            if (ship is null)
            {
                _ships.RemoveAt(i);
                return;
            }

            var (sourceFleet, sourceIndex) = GetSource(ship);

            if (sourceFleet is null)
            {
                if (i < _ships.Count)
                    _ships[i] = ship;
                else
                    _ships.Add(ship);
                return;
            }

            if (sourceFleet == this)
            {
                var oldShip = _ships[i];

                _ships[i] = ship;
                _ships[sourceIndex] = oldShip;
                return;
            }

            if (i < _ships.Count)
            {
                var oldShip = _ships[i];

                sourceFleet._ships.RemoveAt(sourceIndex);
                _ships.RemoveAt(i);

                sourceFleet._ships.Insert(sourceIndex, oldShip);
                _ships.Insert(i, ship);
                return;
            }

            sourceFleet._ships.RemoveAt(sourceIndex);
            _ships.Add(ship);

            (PlayerFleet?, int) GetSource(PlayerShip ship)
            {
                foreach (var fleet in _owner.Fleets)
                {
                    var index = fleet._ships.IndexOf(ship);
                    if (index is -1)
                        continue;

                    return (fleet, index);
                }

                return default;
            }
        }
    }
}
