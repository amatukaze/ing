using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Battle
{
    internal class BattleResultJson : IRawBattleResult
    {
        [JsonProperty("api_win_rank")]
        public BattleRank Rank { get; set; }
        [JsonProperty("api_get_exp")]
        public int AdmiralExperience { get; set; }
        [JsonProperty("api_get_base_exp")]
        public int BaseExperience { get; set; }
        [JsonProperty("api_first_clear")]
        public bool MapCleared { get; set; }

        public EnemyInfo api_enemy_info;
        public string EnemyFleetName => api_enemy_info?.api_deck_name;

        [JsonProperty("api_mapcell_incentive")]
        public bool? AirReconnaissanceSuccessed { get; set; }

        public GetUseItem api_get_useitem;
        public UseItemId? UseItemAcquired => api_get_useitem?.api_useitem_id;

        public GetShip api_get_ship;
        public ShipInfoId? ShipDropped => api_get_ship?.api_ship_id;

        [JsonProperty("api_get_exmap_rate")]
        public int? RankingPointAcquired { get; set; }
        [JsonProperty("api_m1")]
        public bool MapPartUnlocked { get; set; }

        public LandingHP api_landing_hp;
        public int? TransportationPoint => api_landing_hp?.api_sub_value;

        public Escape api_escape;
        public IReadOnlyCollection<int> EscapableShipIndices
        {
            get
            {
                if (api_escape == null)
                    return Array.Empty<int>();

                var l = new List<int>(2);
                if (api_escape.api_escape_idx is int escape)
                    l.Add(escape - 1);
                if (api_escape.api_tow_idx is int tow)
                    l.Add(tow - 1);
                return l;
            }
        }

        public GetEventItem[] api_get_eventitem;
        public IRawRewards Rewards
        {
            get
            {
                if (api_get_eventitem == null)
                    return null;

                var rewards = new Rewards();
                foreach (var r in api_get_eventitem)
                    switch (r.api_type)
                    {
                        case 1:
                            rewards.UseItem.Add(new UseItemRecord
                            {
                                ItemId = (UseItemId)r.api_id,
                                Count = r.api_value
                            });
                            break;
                        case 2:
                            rewards.Ship.Add((ShipInfoId)r.api_id);
                            break;
                        case 3:
                            rewards.Equipment.Add(new EquipmentRecord
                            {
                                Id = (EquipmentInfoId)r.api_id,
                                ImprovementLevel = r.api_slot_level,
                                Count = r.api_value
                            });
                            break;
                        case 5:
                            rewards.Furniture.Add((FurnitureId)r.api_id);
                            break;
                    }
                return rewards;
            }
        }
    }
    internal class EnemyInfo
    {
        public string api_deck_name;
    }
    internal class GetUseItem
    {
        public UseItemId api_useitem_id;
    }
    internal class GetShip
    {
        public ShipInfoId api_ship_id;
    }
    internal class LandingHP
    {
        public int api_sub_value;
    }
    internal class GetEventItem
    {
        public int api_type;
        public int api_id;
        public int api_value;
        public int api_slot_level;
    }
    internal class Escape
    {
        public int? api_escape_idx;
        public int? api_tow_idx;
    }
}
