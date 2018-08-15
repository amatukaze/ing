using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class EquipmentImproveJson
    {
        public bool api_remodel_flag;
        public EquipmentJson api_after_slot;
        public IReadOnlyList<EquipmentId> api_use_slot_id;
    }
}
