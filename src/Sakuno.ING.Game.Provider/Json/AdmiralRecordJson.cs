using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Json
{
    internal class AdmiralRecordJson : IRawAdmiral
    {
        [JsonProperty("api_member_id")]
        public int Id { get; set; }
        [JsonProperty("api_nickname")]
        public string Name { get; set; }
        [JsonProperty("api_cmt")]
        public string Comment { get; set; }

        public int api_level;
        public int[] api_experience;
        public Leveling Leveling => new Leveling(api_level,
            api_experience.ElementAtOrDefault(0),
            KnownLeveling.GetAdmiralExp(api_level),
            api_experience.ElementAtOrDefault(1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        [JsonProperty("api_rank")]
        public AdmiralRank Rank { get; set; }

        public int[] api_ship;
        public int MaxShipCount => api_ship.ElementAtOrDefault(1);

        public int[] api_slotitem;
        public int MaxEquipmentCount => api_slotitem.ElementAtOrDefault(1);

        public RecordBattleStat api_war;
        public BattleStat BattleStat => api_war.ToValue();

        public RecordBattleStat api_practice;
        public BattleStat PracticeStat => api_practice.ToValue();

        public RecordExpeditionStat api_mission;
        public ExpeditionStat ExpeditionStat => api_mission.ToValue();

        [JsonProperty("api_large_dock")]
        public bool CanLSC { get; set; }
        [JsonProperty("api_material_max")]
        public int MaxMaterial { get; set; }
    }

    internal class RecordBattleStat
    {
        public int api_win;
        public int api_lose;
        public int api_rate;
        public BattleStat ToValue()
            => new BattleStat(api_win, api_lose, api_rate);
    }

    internal class RecordExpeditionStat
    {
        public int api_count;
        public int api_success;
        public int api_rate;
        public ExpeditionStat ToValue()
            => new ExpeditionStat(api_success, api_count, api_rate);
    }
}
