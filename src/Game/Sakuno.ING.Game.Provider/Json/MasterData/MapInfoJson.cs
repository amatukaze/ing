using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class MapInfoJson : IRawMapInfo
    {
        [JsonProperty("api_id")]
        public MapId Id { get; set; }
        [JsonProperty("api_maparea_id")]
        public MapAreaId MapAreaId { get; set; }
        [JsonProperty("api_no")]
        public int CategoryNo { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_level")]
        public int StarDifficulty { get; set; }
        [JsonProperty("api_opetext")]
        public string OperationName { get; set; }
        [JsonProperty("api_infotext"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; set; }

        [JsonProperty("api_item")]
        public IReadOnlyCollection<UseItemId> ItemAcquirements { get; set; }
        [JsonProperty("api_required_defeat_count")]
        public int? RequiredDefeatCount { get; set; }

        public int[] api_sally_flag;
        public bool CanUseNormalFleet => api_sally_flag.ElementAtOrDefault(0) != 0;
        public bool CanUseStrikingForceFleet => api_sally_flag.ElementAtOrDefault(2) != 0;
        public bool CanUseCombinedFleet(CombinedFleetType type)
            => (api_sally_flag.ElementAtOrDefault(1) & (1 << ((int)type - 1))) != 0;

        public IRawMapBgmInfo BgmInfo { get; set; }
    }
}
