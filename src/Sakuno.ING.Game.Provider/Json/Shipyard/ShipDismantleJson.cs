using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class ShipDismantleJson:IMaterialsUpdate
    {
        [JsonConverter(typeof(MaterialsConverter))]
        public Materials api_material;

        void IMaterialsUpdate.Apply(ref Materials materials) => materials = api_material;
    }
}
