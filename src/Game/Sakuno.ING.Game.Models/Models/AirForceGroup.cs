using System;
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

        private int _distanceBase;
        public int DistanceBase
        {
            get => _distanceBase;
            set => Set(ref _distanceBase, value);
        }

        private int _distanceBonus;
        public int DistanceBonus
        {
            get => _distanceBonus;
            set => Set(ref _distanceBonus, value);
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

        public AirForceGroup(IRawAirForceGroup raw, NavalBase owner, DateTimeOffset timeStamp) : this(raw.Id, owner) => UpdateProps(raw, timeStamp);

        public event Action<AirForceGroup, IRawAirForceGroup, DateTimeOffset> Updating;
        public override void Update(IRawAirForceGroup raw, DateTimeOffset timeStamp)
        {
            Updating?.Invoke(this, raw, timeStamp);
            UpdateProps(raw, timeStamp);
        }

        private void UpdateProps(IRawAirForceGroup raw, DateTimeOffset timeStamp)
        {
            Name = raw.Name;
            DistanceBase = raw.DistanceBase;
            DistanceBonus = raw.DistanceBonus;
            Action = raw.Action;
            squadrons.BatchUpdate(raw.Squadrons, timeStamp);
        }
    }
}
