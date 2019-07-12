using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public partial class AirForceGroup : BindableObject, IUpdatable<(MapAreaId MapArea, AirForceGroupId GroupId), RawAirForceGroup>
    {
        private IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase> squadrons;
        public ITable<int, AirForceSquadron> Squadrons => squadrons;

        public MapAreaInfo MapArea { get; private set; }

        public AirForceGroupId GroupId { get; private set; }

        partial void CreateCore() => squadrons = new IdTable<int, AirForceSquadron, RawAirForceSquadron, NavalBase>(owner);

        partial void UpdateCore(RawAirForceGroup raw, DateTimeOffset timeStamp)
        {
            GroupId = Id.GroupId;
            MapArea = owner.MasterData.MapAreas[Id.MapArea];
            squadrons.BatchUpdate(raw.Squadrons, timeStamp);
        }

        internal void SetPlane(DateTimeOffset t, AirForceSetPlane msg)
        {
            DistanceBase = msg.NewDistanceBase;
            DistanceBonus = msg.NewDistanceBonus;
            squadrons.BatchUpdate(msg.UpdatedSquadrons, t, removal: false);
        }

        internal void Supply(DateTimeOffset t, IEnumerable<RawAirForceSquadron> updated)
            => squadrons.BatchUpdate(updated, t, removal: false);
    }
}
