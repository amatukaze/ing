using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class ShipTypeInfoJson : IRawShipTypeInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_scnt")]
        public int RepairTimeRatio { get; set; }
        [JsonProperty("api_kcnt")]
        public int BuildOutlineId { get; set; }

        public SortedList<int, bool> api_equip_type;
        public IReadOnlyList<int> AvailableEquipmentTypes
            => api_equip_type
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();
    }
}
