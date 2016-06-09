using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawMapExploration
    {
        [JsonProperty("api_maparea_id")]
        public int AreaID { get; set; }

        [JsonProperty("api_mapinfo_no")]
        public int AreaSubID { get; set; }

        [JsonProperty("api_no")]
        public int Node { get; set; }
        [JsonProperty("api_color_no")]
        public int NodeColor { get; set; }

        [JsonProperty("api_event_id")]
        public SortieEventType NodeEventType { get; set; }
        [JsonProperty("api_event_kind")]
        public int NodeEventSubType { get; set; }

        [JsonProperty("api_next")]
        public int NextRouteCount { get; set; }

        [JsonProperty("api_bosscell_no")]
        public int BossNodeID { get; set; }

        [JsonProperty("api_bosscomp")]
        public int ApiBosscomp { get; set; }

        [JsonProperty("api_comment_kind")]
        public int CommentType { get; set; }
        [JsonProperty("api_production_kind")]
        public int ProductionType { get; set; }

        [JsonProperty("api_eventmap")]
        public RawEventMap EventMap { get; set; }

        [JsonProperty("api_from_no")]
        public int? StartNode { get; set; }

        [JsonProperty("api_happening")]
        public WhirlpoolEvent Whirlpool { get; set; }

        [JsonProperty("api_itemget")]
        public JToken Rewards { get; set; }
        [JsonProperty("api_get_eo_rate")]
        public int RankingPointBonus { get; set; }
        [JsonProperty("api_itemget_eo_comment")]
        public RawReward RewardInExtraOperation { get; set; }

        [JsonProperty("api_select_route")]
        public RawNodeSelection NodeSelection { get; set; }

        [JsonProperty("api_airsearch")]
        public RawAviationReconnaissance AviationReconnaissance { get; set; }

        public class RawEventMap
        {
            [JsonProperty("api_now_maphp")]
            public int Current { get; set; }
            [JsonProperty("api_max_maphp")]
            public int Maximum { get; set; }
        }
        public class WhirlpoolEvent
        {
            [JsonProperty("api_count")]
            public int Amount { get; set; }

            [JsonProperty("api_mst_id")]
            public MaterialType MaterialType { get; set; }

            [JsonProperty("api_icon_id")]
            public MaterialType IconID { get; set; }

            [JsonProperty("api_dentan")]
            public bool HasReduceLossesWithRadar { get; set; }
        }
        public class RawReward : ModelBase
        {
            [JsonProperty("api_usemst")]
            public int TypeID { get; set; }
            [JsonProperty("api_id")]
            public MaterialType ID { get; set; }

            [JsonProperty("api_getcount")]
            public int Quantity { get; set; }
        }
        public class RawNodeSelection
        {
            [JsonProperty("api_select_cells")]
            public int[] Nodes { get; set; }
        }
        public class RawAviationReconnaissance
        {
            [JsonProperty("api_plane_type")]
            public AviationReconnaissancePlaneType PlaneType { get; set; }

            [JsonProperty("api_result")]
            public AviationReconnaissanceResult Result { get; set; }
        }

    }
}
