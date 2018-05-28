using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyCollection<FleetType> AvailableFleetTypes
        {
            get
            {
                var list = new List<FleetType>(5);

                if (api_sally_flag.ElementAtOrDefault(0) != 0)
                    list.Add(FleetType.SingleFleet);
                int combined = api_sally_flag.ElementAtOrDefault(1);
                if ((combined & 1) != 0)
                    list.Add(FleetType.CarrierTaskForceFleet);
                if ((combined & 2) != 0)
                    list.Add(FleetType.SurfaceTaskForceFleet);
                if ((combined & 4) != 0)
                    list.Add(FleetType.TransportEscortFleet);
                if (api_sally_flag.ElementAtOrDefault(2) != 0)
                    list.Add(FleetType.StrikingForceFleet);

                return list;
            }
        }

        public IRawMapBgmInfo BgmInfo { get; set; }
    }
}
