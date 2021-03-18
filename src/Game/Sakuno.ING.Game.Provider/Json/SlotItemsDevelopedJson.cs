using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class SlotItemsDevelopedJson : IMaterialUpdate
    {
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool api_create_flag { get; set; }
        public DevelopedSlotItemJson[] api_get_items { get; set; }
        public Materials api_material { get; set; }

        public void Apply(Materials materials)
        {
            materials.Fuel = api_material.Fuel;
            materials.Bullet = api_material.Bullet;
            materials.Steel = api_material.Steel;
            materials.Bauxite = api_material.Bauxite;
            materials.InstantBuild = api_material.InstantBuild;
            materials.InstantRepair = api_material.InstantRepair;
            materials.Development = api_material.Development;
            materials.Improvement = api_material.Improvement;
        }

        public sealed class DevelopedSlotItemJson
        {
            public int api_id { get; set; }
            public int api_slotitem_id { get; set; }
        }
    }
#nullable enable
}
