using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class EquipmentCreationJson : IMaterialsUpdate
    {
        public bool api_create_flag;
        public CreatedEquipmentJson[] api_get_items;
        [JsonConverter(typeof(MaterialsConverter))]
        public Materials api_material;

        public class CreatedEquipmentJson
        {
            internal int api_id;
            internal int api_slotitem_id;
        }

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.EquipmentCreate;

        void IMaterialsUpdate.Apply(ref Materials materials) => materials = api_material;
    }
}
