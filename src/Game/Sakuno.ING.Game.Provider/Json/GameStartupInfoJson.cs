using System.Collections.Generic;

namespace Sakuno.ING.Game.Json
{
    internal class GameStartupInfoJson
    {
        public EquipmentJson[] api_slot_item;
        public BuildingDockJson[] api_kdock;
        public UseItemCountJson[] api_useitem;
        public Dictionary<string, int[]> api_unsetslot;
    }
}
