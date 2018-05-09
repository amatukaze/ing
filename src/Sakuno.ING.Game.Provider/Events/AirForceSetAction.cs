using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public struct AirForceSetAction
    {
        public int MapAreaId;
        public int AirForceId;
        public AirForceAction Action;
    }
}
