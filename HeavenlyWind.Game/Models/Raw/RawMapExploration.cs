using Newtonsoft.Json;
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
        public int Cell { get; set; }
        [JsonProperty("api_color_no")]
        public int CellColor { get; set; }

        [JsonProperty("api_event_id")]
        public SortieEventType CellEventType { get; set; }
        [JsonProperty("api_event_kind")]
        public int CellEventSubType { get; set; }

        [JsonProperty("api_next")]
        public int NextRouteCount { get; set; }

        [JsonProperty("api_bosscell_no")]
        public int BossCellNo { get; set; }

        [JsonProperty("api_bosscomp")]
        public int ApiBosscomp { get; set; }

        [JsonProperty("api_comment_kind")]
        public int CommentType { get; set; }
        [JsonProperty("api_production_kind")]
        public int ProductionType { get; set; }

        [JsonProperty("api_eventmap")]
        public RawEventMap EventMap { get; set; }

        [JsonProperty("api_happening")]
        public WhirlpoolEvent Whirlpool { get; set; }

        [JsonProperty("api_itemget")]
        public RawReward Reward { get; set; }
        [JsonProperty("api_itemget_eo_comment")]
        public RawReward RewardInExtraOperation { get; set; }

        [JsonProperty("api_select_route")]
        public RawRouteSelection RouteSelections { get; set; }

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
            public SortieItem MaterialType { get; set; }

            [JsonProperty("api_icon_id")]
            public SortieItem IconID { get; set; }

            [JsonProperty("api_dentan")]
            public bool IsReducedByRadar { get; set; }
        }
        public class RawReward
        {
            [JsonProperty("api_usemst")]
            public int TypeID { get; set; }
            [JsonProperty("api_id")]
            public SortieItem Item { get; set; }

            [JsonProperty("api_getcount")]
            public int Quantity { get; set; }
        }
        public class RawRouteSelection
        {
            [JsonProperty("api_select_cells")]
            public int[] Cells { get; set; }
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
