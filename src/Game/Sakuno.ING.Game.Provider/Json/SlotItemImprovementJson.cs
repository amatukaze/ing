using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class SlotItemImprovementJson : IMaterialUpdate
    {
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool api_remodel_flag { get; set; }
        public Materials api_after_material { get; set; }
        public RawSlotItem api_after_slot { get; set; }
        public IReadOnlyList<SlotItemId> api_use_slot_id { get; set; }

        public void Apply(Materials materials)
        {
            materials.Fuel = api_after_material.Fuel;
            materials.Bullet = api_after_material.Bullet;
            materials.Steel = api_after_material.Steel;
            materials.Bauxite = api_after_material.Bauxite;
            materials.InstantBuild = api_after_material.InstantBuild;
            materials.InstantRepair = api_after_material.InstantRepair;
            materials.Development = api_after_material.Development;
            materials.Improvement = api_after_material.Improvement;
        }
    }
#nullable enable
}
