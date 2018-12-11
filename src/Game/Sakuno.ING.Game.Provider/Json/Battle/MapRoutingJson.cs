using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    internal class MapRoutingJson : IRawMapRouting
    {
        public int api_maparea_id;
        public int api_mapinfo_no;
        public MapId MapId => (MapId)(api_maparea_id * 10 + api_mapinfo_no);

        [JsonProperty("api_no")]
        public int RouteId { get; set; }

        [JsonProperty("api_event_id")]
        public MapEventKind EventKind { get; set; }
        [JsonProperty("api_event_kind")]
        public BattleKind BattleKind { get; set; }
        [JsonProperty("api_next")]
        public bool CanMoveAdvance { get; set; }
        [JsonProperty("api_production_kind")]
        public bool Reconnaissance { get; set; }

        public class CellFlavor
        {
            public int api_type;
            public string api_message;
        }
        public CellFlavor api_cell_flavor;
        public string Message => api_cell_flavor?.api_message;

        [JsonProperty("api_select_route")]
        public int[] SelectableRoutes { get; set; }
        public bool CanSelectRoute => SelectableRoutes != null;

        public class ItemGet
        {
            public int api_usemst;
            public int api_id;
            public int api_getcount;
        }
        public ItemGet[] api_itemget;
        public class ItemLose
        {
            public int api_usemst;
            public int api_type;
            public int api_mst_id;
            public int api_count;
        }
        public ItemLose api_happening;
        public ItemGet api_itemget_eo_comment;
        private UseItemId IconIdToItemId(int type, int iconId)
        {
            if (type == 4)
            {
                switch (iconId)
                {
                    case 1:
                        return (UseItemId)31;
                    case 2:
                        return (UseItemId)32;
                    case 3:
                        return (UseItemId)33;
                    case 4:
                        return (UseItemId)34;
                    case 5:
                        return (UseItemId)2;
                    case 6:
                        return (UseItemId)1;
                    case 7:
                        return (UseItemId)3;
                    case 8:
                        return (UseItemId)4;
                    default:
                        return default;
                }
            }
            else if (type == 5)
                return (UseItemId)iconId;
            else return default;
        }
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
        public int? RankingPointAcquired { get; set; }
    }
}
