using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class AdmiralJson : IRawAdmiral
    {
        [JsonProperty("api_member_id")]
        public int Id { get; set; }
        [JsonProperty("api_nickname")]
        public string Name { get; set; }

        public int api_level;
        public int api_experience;
        public Leveling Leveling => new Leveling(api_level,
            api_experience,
            KnownLeveling.GetAdmiralExp(api_level),
            KnownLeveling.GetAdmiralExp(api_level + 1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        [JsonProperty("api_rank")]
        public AdmiralRank Rank { get; set; }
        [JsonProperty("api_comment")]
        public string Comment { get; set; }
        [JsonProperty("api_max_chara")]
        public int MaxShipCount { get; set; }
        [JsonProperty("api_max_slotitem")]
        public int MaxEquipmentCount { get; set; }

        public int api_st_win;
        public int api_st_lose;
        public int api_ms_count;
        public int api_ms_success;
        public int api_pt_win;
        public int api_pt_lose;

        public BattleStat BattleStat => new BattleStat(api_st_win, api_st_lose);
        public BattleStat PracticeStat => new BattleStat(api_pt_win, api_pt_lose);
        public ExpeditionStat ExpeditionStat => new ExpeditionStat(api_ms_success, api_ms_count);

        [JsonProperty("api_large_dock")]
        public bool CanLSC { get; set; }
        public int MaxMaterial => Leveling.Level * 250 + 750;
    }
}
