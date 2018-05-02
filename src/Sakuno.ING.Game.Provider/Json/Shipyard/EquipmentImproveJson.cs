using System;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class EquipmentImproveJson
    {
        public bool api_remodel_flag;
        public EquipmentJson api_after_slot;
        public int[] api_use_slot_id = Array.Empty<int>();
    }
}
