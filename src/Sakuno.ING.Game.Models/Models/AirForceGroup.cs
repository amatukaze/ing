using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class AirForceGroup : Calculated<(MapAreaId MapArea, AirForceGroupId GroupId), IRawAirForceGroup>
    {
        internal readonly IdTable<int, AirForceSquadron, IRawAirForceSquadron, NavalBase> squadrons;
        public ITable<int, AirForceSquadron> Squadrons => squadrons;

        private readonly NavalBase owner;

        public MapAreaInfo MapArea { get; }

        public AirForceGroupId GroupId { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private int _distance;
        public int Distance
        {
            get => _distance;
            set => Set(ref _distance, value);
        }

        private AirForceAction _action;
        public AirForceAction Action
        {
            get => _action;
            set => Set(ref _action, value);
        }

        public AirForceGroup((MapAreaId MapArea, AirForceGroupId GroupId) id, NavalBase owner)
            : base(id)
        {
            this.owner = owner;
            GroupId = id.GroupId;
            MapArea = owner.MasterData.MapAreas[id.MapArea];
            squadrons = new IdTable<int, AirForceSquadron, IRawAirForceSquadron, NavalBase>(owner);
        }

        public override void Update(IRawAirForceGroup raw)
        {
            Name = raw.Name;
            Distance = raw.Distance;
            Action = raw.Action;
            squadrons.BatchUpdate(raw.Squadrons);
        }
    }
}
