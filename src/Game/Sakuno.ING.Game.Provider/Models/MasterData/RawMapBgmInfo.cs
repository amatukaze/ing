using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawMapBgmInfo
    {
        [JsonPropertyName("api_id")]
        public MapId Id { get; set; }
        [JsonPropertyName("api_moving_bgm")]
        public int MapBgmId { get; set; }

        public int[] api_map_bgm { get; set; }
        public int[] api_boss_bgm { get; set; }
        public int NormalBattleDayBgmId => api_map_bgm[0];
        public int NormalBattleNightBgmId => api_map_bgm[1];
        public int BossBattleDayBgmId => api_boss_bgm[0];
        public int BossBattleNightBgmId => api_boss_bgm[1];
    }
#nullable enable
}
