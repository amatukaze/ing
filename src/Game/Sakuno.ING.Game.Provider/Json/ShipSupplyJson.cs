using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class ShipSupplyJson : IShipSupply
    {
        [JsonProperty("api_id")]
        public ShipId ShipId { get; set; }
        [JsonProperty("api_fuel")]
        public int CurrentFuel { get; set; }
        [JsonProperty("api_bull")]
        public int CurrentBullet { get; set; }
        [JsonProperty("api_onslot")]
        public IReadOnlyList<int> SlotAircraft { get; set; }
    }

    internal class ShipsSupplyJson : IMaterialsUpdate
    {
        public ShipSupplyJson[] api_ship;
        [JsonConverter(typeof(MaterialsConverter))]
        public Materials api_material;

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.ShipSupply;

        void IMaterialsUpdate.Apply(ref Materials materials)
        {
            materials.Fuel = api_material.Fuel;
            materials.Bullet = api_material.Bullet;
            materials.Steel = api_material.Steel;
            materials.Bauxite = api_material.Bauxite;
        }
    }
}
