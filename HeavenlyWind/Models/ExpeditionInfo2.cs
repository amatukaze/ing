using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class ExpeditionInfo2 : IID
    {
        [JsonProperty("id")]
        public int ID { get; internal set; }

        [JsonProperty("resources")]
        public RawResources Resources { get; internal set; }

        [JsonProperty("flagship_lv")]
        public int FlagshipLevel { get; internal set; }
        [JsonProperty("flagship_type")]
        public int? FlagshipType { get; internal set; }

        [JsonProperty("total_lv")]
        public int? TotalLevel { get; internal set; }

        [JsonProperty("ship_count")]
        public int ShipCount { get; internal set; }

        [JsonProperty("ship_requirement")]
        public RawShipRequirement[] ShipRequirements { get; internal set; }

        [JsonProperty("drum")]
        public RawDrum DrumRequirement { get; internal set; }

        public class RawResources
        {
            [JsonProperty("bullet")]
            public int Bullet { get; internal set; }
            [JsonProperty("steel")]
            public int Steel { get; internal set; }
            [JsonProperty("fuel")]
            public int Fuel { get; internal set; }
            [JsonProperty("bauxite")]
            public int Bauxite { get; internal set; }
        }
        public class RawShipRequirement
        {
            [JsonProperty("count")]
            public int Count { get; internal set; }
            [JsonProperty("types")]
            public int[] Types { get; internal set; }
        }
        public class RawDrum
        {
            [JsonProperty("count")]
            public int Count { get; internal set; }
            [JsonProperty("ship_count")]
            public int ShipCount { get; internal set; }
        }
    }
}
