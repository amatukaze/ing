using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class EquipmentImproveJson
    {
        public bool api_remodel_flag;
        public EquipmentJson api_after_slot;
        public EquipmentId[] api_use_slot_id = Array.Empty<EquipmentId>();
    }
}
