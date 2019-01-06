using Newtonsoft.Json;

namespace Sakuno.ING.Game.Models.MasterData
{
    public sealed class RawMapBgmInfo
    {
        internal RawMapBgmInfo() { }

        [JsonProperty("api_id")]
        public int Id { get; internal set; }

        [JsonProperty("api_moving_bgm")]
        public int MapBgmId { get; internal set; }

        internal int[] api_map_bgm;
        internal int[] api_boss_bgm;
        public int NormalBattleDayBgmId => api_map_bgm.ElementAtOrDefault(0);
        public int NormalBattleNightBgmId => api_map_bgm.ElementAtOrDefault(1);
        public int BossBattleDayBgmId => api_boss_bgm.ElementAtOrDefault(0);
        public int BossBattleNightBgmId => api_boss_bgm.ElementAtOrDefault(1);
    }
}
