using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class ShipBuildCompletionJson
    {
        public RawBuildingDock[] api_kdock;
        public RawShip api_ship;
        public RawEquipment[] api_slotitem;
    }
}
