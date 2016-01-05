using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawBattleResult
    {
        [JsonProperty("api_ship_id")]
        public int[] ShipIDs { get; set; }

        [JsonProperty("api_win_rank")]
        public BattleRank Rank { get; set; }

        [JsonProperty("api_get_exp")]
        public int AdmiralExperienceIncrement { get; set; }

        [JsonProperty("api_mvp")]
        public int MvpShipIndex { get; set; }

        [JsonProperty("api_member_lv")]
        public int HeadquarterLevel { get; set; }
        [JsonProperty("api_member_exp")]
        public int AdmiralExperience { get; set; }

        [JsonProperty("api_get_base_exp")]
        public int BaseExperience { get; set; }

        [JsonProperty("api_get_ship_exp")]
        public int[] ExperienceIncrement { get; set; }
        [JsonProperty("api_get_ship_exp_combined")]
        public int[] EscortFleetExperienceIncrement { get; set; }

        [JsonProperty("api_get_exp_lvup")]
        public int[][] ExperienceToNextLevel { get; set; }
        [JsonProperty("api_get_exp_lvup_combined")]
        public int[][] EscortFleetExperienceToNextLevel { get; set; }

        [JsonProperty("api_dests")]
        public int SunkEnemyCount { get; set; }
        [JsonProperty("api_destsf")]
        public bool IsFlagshipSunk { get; set; }

        [JsonProperty("api_lost_flag")]
        public int[] IsFriendShipSunk { get; set; }

        [JsonProperty("api_quest_name")]
        public string MapName { get; set; }
        [JsonProperty("api_quest_level")]
        public int MapLevel { get; set; }

        [JsonProperty("api_enemy_info")]
        public RawEnemyInfo EnemyInfo { get; set; }

        [JsonProperty("api_first_clear")]
        public bool IsFirstClear { get; set; }

        [JsonProperty("api_get_flag")]
        public bool[] IsDropped { get; set; }

        [JsonProperty("api_get_useitem")]
        public RawDroppedItem DroppedItem { get; set; }
        [JsonProperty("api_get_ship")]
        public RawDroppedShip DroppedShip { get; set; }

        [JsonProperty("api_get_exmap_rate")]
        public int ExtraOperationRankingPointBonus { get; set; }

        public RawEventMapBouns[] EventMapBouns { get; set; }

        [JsonProperty("api_escape_flag")]
        public bool HasEscapedShip { get; set; }

        public class RawEnemyInfo
        {
            [JsonProperty("api_level")]
            public string Level { get; set; }

            [JsonProperty("api_rank")]
            public string Rank { get; set; }

            [JsonProperty("api_deck_name")]
            public string FleetName { get; set; }
        }
        public class RawDroppedItem
        {
            [JsonProperty("api_useitem_id")]
            public int ID { get; set; }
        }
        public class RawDroppedShip
        {
            [JsonProperty("api_ship_id")]
            public int ID { get; set; }

            [JsonProperty("api_ship_type")]
            public string Type { get; set; }

            [JsonProperty("api_ship_name")]
            public string Name { get; set; }
        }
        public class RawEventMapBouns
        {
            [JsonProperty("api_type")]
            public EventMapBounsType Type { get; set; }

            [JsonProperty("api_id")]
            public int ID { get; set; }

            [JsonProperty("api_value")]
            public int Count { get; set; }
        }
        public class RawEscapedShip
        {
            [JsonProperty("api_escape_idx")]
            public int[] EscapedShipID { get; set; }

            [JsonProperty("api_tow_idx")]
            public int[] EscortShipID { get; set; }
        }
    }
}