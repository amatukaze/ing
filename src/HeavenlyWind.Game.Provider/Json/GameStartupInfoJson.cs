using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class GameStartupInfoJson
    {
        public EquipmentJson[] api_slot_item;
        public BuildingDockJson[] api_kdock;
        public UseItemCountJson[] api_useitem;
        public IDictionary<string, int[]> api_unsetslot;
    }
}
