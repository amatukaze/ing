using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawOpeningTorpedoSalvoPhase
    {
        //[JsonProperty("api_frai")]

        //[JsonProperty("api_erai")]

        [JsonProperty("api_fdam")]
        public int[] FriendDamage { get; set; }

        [JsonProperty("api_edam")]
        public int[] EnemyDamage { get; set; }

        [JsonProperty("api_fydam_list_items")]
        public int[][] FriendDamageGivenToOpponent { get; set; }

        [JsonProperty("api_eydam_list_items")]
        public int[][] EnemyDamageGivenToOpponent { get; set; }

        //[JsonProperty("api_fcl")]

        //[JsonProperty("api_ecl")]
    }
}
