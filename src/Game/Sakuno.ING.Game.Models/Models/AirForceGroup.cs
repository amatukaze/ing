using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public partial class AirForceGroup
    {
        public MapAreaInfo Area { get; private set; }

        private IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase> _squadrons;
        public ITable<int, AirForceSquadron> Squadrons => _squadrons;

        partial void CreateCore()
        {
            Area = _owner.MasterData.MapAreas[Id.MapArea];

            _squadrons = new IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase>(_owner);
        }

        partial void UpdateCore(RawAirForceGroup raw)
        {
            _squadrons.BatchUpdate(raw.Squadrons);
        }
    }
}
