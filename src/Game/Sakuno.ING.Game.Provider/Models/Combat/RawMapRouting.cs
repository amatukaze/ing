using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public sealed class RawMapRouting
    {
        internal int api_maparea_id;
        internal int api_mapinfo_no;
        public MapId MapId => (MapId)(api_maparea_id * 10 + api_mapinfo_no);

        [JsonProperty("api_no")]
        public int RouteId { get; internal set; }

        [JsonProperty("api_event_id")]
        public MapEventKind EventKind { get; internal set; }
        [JsonProperty("api_event_kind")]
        public BattleKind BattleKind { get; internal set; }
        [JsonProperty("api_next")]
        public bool CanMoveAdvance { get; internal set; }
        [JsonProperty("api_production_kind")]
        public bool Reconnaissance { get; internal set; }

        internal class CellFlavor
        {
            public int api_type;
            public string api_message;
        }
        internal CellFlavor api_cell_flavor;
        public string Message => api_cell_flavor?.api_message;

        internal class SelectRoute
        {
            public int[] api_select_cells;
        }
        internal SelectRoute api_select_route;
        public IReadOnlyCollection<int> SelectableRoutes => api_select_route?.api_select_cells;

        internal class ItemGet
        {
            public int api_usemst;
            public int api_id;
            public int api_getcount;
        }
        internal ItemGet[] api_itemget;
        internal class ItemLose
        {
            public int api_usemst;
            public int api_type;
            public int api_mst_id;
            public int api_count;
        }
        internal ItemLose api_happening;
        internal ItemGet api_itemget_eo_comment;
        private UseItemId IconIdToItemId(int type, int iconId) => (type, iconId) switch
        {
            (4, 1) => (UseItemId)31,
            (4, 2) => (UseItemId)32,
            (4, 3) => (UseItemId)33,
            (4, 4) => (UseItemId)34,
            (4, 5) => (UseItemId)2,
            (4, 6) => (UseItemId)1,
            (4, 7) => (UseItemId)3,
            (4, 8) => (UseItemId)4,
            (5, _) => (UseItemId)iconId,
            _ => default
        };
        private UseItemRecord ParseItemGet(ItemGet api)
            => new UseItemRecord
            {
                ItemId = IconIdToItemId(api.api_usemst, api.api_id),
                Count = api.api_getcount
            };
        public IReadOnlyCollection<UseItemRecord> ItemAcquired
            => api_itemget_eo_comment != null ?
                new[] { ParseItemGet(api_itemget_eo_comment) } :
                api_itemget?.Select(ParseItemGet).ToArray() ?? Array.Empty<UseItemRecord>();
        public IReadOnlyCollection<UseItemRecord> ItemLost
            => api_happening != null ?
                new[] { new UseItemRecord
                {
                    ItemId = IconIdToItemId(api_happening.api_type, api_happening.api_mst_id),
                    Count = api_happening.api_count
                } } :
                Array.Empty<UseItemRecord>();

        [JsonProperty("api_get_eo_rate")]
        public int? RankingPointAcquired { get; internal set; }

        [JsonProperty("api_destruction_battle")]
        public JToken UnparsedLandBaseDefence { get; internal set; }
        public RawBattle LandBaseDefence { get; internal set; }

        internal bool api_m1;
    }
}
