using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class AirForceGroup : BindableObject, IUpdatable<(MapAreaId MapArea, AirForceGroupId GroupId), RawAirForceGroup>
    {
        internal readonly IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase> squadrons;
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

        public (MapAreaId MapArea, AirForceGroupId GroupId) Id { get; }
        public DateTimeOffset UpdationTime { get; }

        public AirForceGroup((MapAreaId MapArea, AirForceGroupId GroupId) id, NavalBase owner)
        {
            Id = id;
            this.owner = owner;
            GroupId = id.GroupId;
            MapArea = owner.MasterData.MapAreas[id.MapArea];
            squadrons = new IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase>(owner);
        }

        public AirForceGroup(RawAirForceGroup raw, NavalBase owner, DateTimeOffset timeStamp) : this(raw.Id, owner) => UpdateProps(raw, timeStamp);

        public event Action<AirForceGroup, RawAirForceGroup, DateTimeOffset> Updating;
        public void Update(RawAirForceGroup raw, DateTimeOffset timeStamp)
        {
            Updating?.Invoke(this, raw, timeStamp);
            UpdateProps(raw, timeStamp);
        }

        private void UpdateProps(RawAirForceGroup raw, DateTimeOffset timeStamp)
        {
            Name = raw.Name;
            DistanceBase = raw.DistanceBase;
            DistanceBonus = raw.DistanceBonus;
            Action = raw.Action;
            squadrons.BatchUpdate(raw.Squadrons, timeStamp);
        }
    }
}
