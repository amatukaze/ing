using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public readonly struct AirForceSetAction
    {
        public AirForceSetAction(MapAreaId mapAreaId, AirForceGroupId groupId, AirForceAction action)
        {
            MapAreaId = mapAreaId;
            GroupId = groupId;
            Action = action;
        }

        public MapAreaId MapAreaId { get; }
        public AirForceGroupId GroupId { get; }
        public AirForceAction Action { get; }
    }
}
