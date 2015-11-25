using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawAerialCombatPhase
    {
        [JsonProperty("api_plane_from")]
        public int[][] Attackers { get; set; }

        [JsonProperty("api_stage1")]
        public RawStage1 Stage1 { get; set; }

        [JsonProperty("api_stage2")]
        public RawStage2 Stage2 { get; set; }

        [JsonProperty("api_stage3")]
        public RawStage3 Stage3 { get; set; }
        [JsonProperty("api_stage3_combined")]
        public RawStage3 Stage3CombinedFleet { get; set; }

        public class RawStage1
        {
            [JsonProperty("api_f_count")]
            public int FriendPlaneCount { get; set; }
            [JsonProperty("api_f_lostcount")]
            public int FriendPlaneLostCount { get; set; }

            [JsonProperty("api_e_count")]
            public int EnemyPlaneCount { get; set; }
            [JsonProperty("api_e_lostcount")]
            public int EnemyPlaneLostCount { get; set; }

            [JsonProperty("api_disp_seiku")]
            public AerialCombatResult Result { get; set; }

            [JsonProperty("api_touch_plane")]
            public int[] ContactPlane { get; set; }
        }
        public class RawStage2
        {
            [JsonProperty("api_f_count")]
            public int FriendPlaneCount { get; set; }
            [JsonProperty("api_f_lostcount")]
            public int FriendPlaneLostCount { get; set; }

            [JsonProperty("api_e_count")]
            public int EnemyPlaneCount { get; set; }
            [JsonProperty("api_e_lostcount")]
            public int EnemyPlaneLostCount { get; set; }
        }
        public class RawStage3
        {
            //[JsonProperty("api_frai_flag")]

            //[JsonProperty("api_erai_flag")]

            //[JsonProperty("api_fbak_flag")]

            //[JsonProperty("api_ebak_flag")]

            //[JsonProperty("api_fcl_flag")]

            //[JsonProperty("api_ecl_flag")]

            [JsonProperty("api_fdam")]
            public int[] FriendDamage { get; set; }

            [JsonProperty("api_edam")]
            public int[] EnemyDamage { get; set; }
        }
    }
}
