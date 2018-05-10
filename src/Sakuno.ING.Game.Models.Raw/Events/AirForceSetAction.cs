using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public readonly struct AirForceSetAction
    {
        public AirForceSetAction(int mapAreaId, int airForceId, AirForceAction action)
        {
            MapAreaId = mapAreaId;
            AirForceId = airForceId;
            Action = action;
        }

        public int MapAreaId { get; }
        public int AirForceId { get; }
        public AirForceAction Action { get; }
    }
}
