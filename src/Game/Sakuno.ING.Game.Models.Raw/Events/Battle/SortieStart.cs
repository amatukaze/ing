using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.Battle
{
    public readonly struct SortieStart
    {
        public SortieStart(FleetId fleetId, MapId mapId)
        {
            FleetId = fleetId;
            MapId = mapId;
        }

        public FleetId FleetId { get; }
        public MapId MapId { get; }
    }
}
