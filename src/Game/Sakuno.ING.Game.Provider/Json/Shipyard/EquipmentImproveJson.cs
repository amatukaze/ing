using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class EquipmentImproveJson : IMaterialsUpdate
    {
        public bool api_remodel_flag;
        [JsonConverter(typeof(MaterialsConverter))]
        public Materials api_after_material;
        public RawEquipment api_after_slot;
        public IReadOnlyList<EquipmentId> api_use_slot_id;

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.EquipmentImprove;

        void IMaterialsUpdate.Apply(ref Materials materials) => materials = api_after_material;
    }
}
