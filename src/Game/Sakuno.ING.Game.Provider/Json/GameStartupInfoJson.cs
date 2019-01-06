using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class GameStartupInfoJson
    {
        public RawEquipment[] api_slot_item;
        public RawBuildingDock[] api_kdock;
        public RawUseItemCount[] api_useitem;
        public Dictionary<string, int[]> api_unsetslot;
    }
}
