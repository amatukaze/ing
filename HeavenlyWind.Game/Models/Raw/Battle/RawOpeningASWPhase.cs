using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawOpeningASWPhase
    {
        [JsonProperty("api_at_list")]
        public int[] Attackers { get; set; }

        [JsonProperty("api_df_list")]
        public object[] Defenders { get; set; }

        //[JsonProperty("api_si_list")]

        //[JsonProperty("api_cl_list")]

        //[JsonProperty("api_sp_list")]

        [JsonProperty("api_damage")]
        public object[] Damages { get; set; }
    }
}
