using DynamicData;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.Game.Models
{
    public partial class PlayerFleet
    {
        private SourceList<PlayerShip> _ships = new SourceList<PlayerShip>();
        public IObservable<IChangeSet<PlayerShip>> Ships { get; private set; }

        partial void CreateCore()
        {
            Ships = _ships.Connect();
        }

        partial void UpdateCore(RawFleet raw)
        {
            ExpeditionState = raw.ExpeditionStatus.State;
            Expedition = _owner.MasterData.Expeditions[raw.ExpeditionStatus.ExpeditionId];

            _ships.EditDiff(raw.ShipIds.Where(id => id > -1).Select(id => _owner.Ships[id]));
        }
    }
}
