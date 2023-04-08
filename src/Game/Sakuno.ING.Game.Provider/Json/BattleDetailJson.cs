using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Json
{
    public class BattleDetailJson
    {
        public int[] api_active_deck;
        public int[] api_ship_ke;
        public int[] api_ship_ke_combined;
        public int[] api_ship_lv;
        public int[] api_ship_lv_combined;

        #region Deprecated in 17Q4
        public int[] api_nowhps;
        public int[] api_maxhps;
        public int[] api_nowhps_combined;
        public int[] api_maxhps_combined;
        #endregion

        #region Deprecated in 15Q2
        public int[][] api_eKyouka;
        #endregion

        public int[] api_f_nowhps;
        public int[] api_f_maxhps;
        public int[] api_f_nowhps_combined;
        public int[] api_f_maxhps_combined;
        [JsonConverter(typeof(HPNaConverter))]
        public int[] api_e_nowhps;
        [JsonConverter(typeof(HPNaConverter))]
        public int[] api_e_maxhps;
        [JsonConverter(typeof(HPNaConverter))]
        public int[] api_e_nowhps_combined;
        [JsonConverter(typeof(HPNaConverter))]
        public int[] api_e_maxhps_combined;

        public bool api_midnight_flag;
        public int[][] api_eSlot;
        public int[][] api_eSlot_combined;
        public int[][] api_fParam;
        public int[][] api_eParam;
        public int[][] api_fParam_combined;
        public int[][] api_eParam_combined;
        public int[] api_escape_idx;
        public int[] api_escape_idx_combined;
        public Detection[] api_search;
        public int[] api_formation;
        public class LandBase : Aerial
        {
            public int api_base_id;
            public class Squadron
            {
                public int api_mst_id;
                public int api_count;
            }
            public Squadron[] api_squadron_plane;
        }
        public Aerial api_air_base_injection;
        public Aerial api_injection_kouku;
        public LandBase[] api_air_base_attack;
        public class Aerial
        {
            public int[][] api_plane_from;
            public class Stage1
            {
                public int api_f_count;
                public int api_f_lostcount;
                public int api_e_count;
                public int api_e_lostcount;
                public int? api_disp_seiku;
                public int[] api_touch_plane;
            }
            public Stage1 api_stage1;
            public class Stage2
            {
                public int api_f_count;
                public int api_f_lostcount;
                public int api_e_count;
                public int api_e_lostcount;
                public class AAFire
                {
                    public int api_idx;
                    public int api_kind;
                    public int[] api_use_items;
                }
                public AAFire api_air_fire;
            }
            public Stage2 api_stage2;
            public class Stage3
            {
                public bool[] api_erai_flag;
                public bool[] api_ebak_flag;
                public int[] api_ecl_flag;
                public decimal[] api_edam;
                public bool[] api_frai_flag;
                public bool[] api_fbak_flag;
                public int[] api_fcl_flag;
                public decimal[] api_fdam;
            }
            public Stage3 api_stage3;
            public Stage3 api_stage3_combined;
        }
        public Aerial api_kouku;
        public Aerial api_kouku2;
        public SupportFireType api_support_flag;
        public class Support
        {
            public class AerialSupport : Aerial
            {
                public int[] api_ship_id;
            }
            public AerialSupport api_support_airatack;
            public class ShellingSupport
            {
                public int[] api_ship_id;
                public int[] api_cl_list;
                public decimal[] api_damage;
            }
            public ShellingSupport api_support_hourai;
        }
        public Support api_support_info;
        public SupportFireType api_n_support_flag;
        public Support api_n_support_info;
        public Shelling api_opening_taisen;
        public class Torpedo
        {
            public int[] api_frai;
            public int[] api_erai;
            public decimal[] api_fdam;
            public decimal[] api_edam;
            public decimal[] api_fydam;
            public decimal[] api_eydam;
            public int[] api_fcl;
            public int[] api_ecl;
        }
        public Torpedo api_opening_atack;
        public class Shelling
        {
            public bool[] api_at_eflag;
            public int[] api_at_list;
            public int[] api_at_type;
            public int[] api_sp_list;
            public int[][] api_df_list;
            public int[][] api_si_list;
            public int[][] api_cl_list;
            public decimal[][] api_damage;
        }
        public Shelling api_hougeki1;
        public Shelling api_hougeki2;
        public Shelling api_hougeki3;
        public Torpedo api_raigeki;
        public int[] api_touch_plane;
        public int[] api_flare_pos;
        public Shelling api_hougeki;
        public Shelling api_n_hougeki1;
        public Shelling api_n_hougeki2;
        public class FriendInfo
        {
            public int api_production_type;
            public int[] api_ship_id;
            public int[] api_ship_lv;
            public int[] api_nowhps;
            public int[] api_maxhps;
            public int[][] api_Slot;
            public int[][] api_Param;
        }
        public FriendInfo api_friendly_info;
        public class FriendBattle
        {
            public int[] api_flare_pos;
            public Shelling api_hougeki;
        }
        public FriendBattle api_friendly_battle;
    }

    public class LandBaseDefenceDetailsJson
    {
        public int[] api_formation;
        public int[] api_ship_ke;
        public int[] api_ship_lv;

        #region Deprecated in 17Q4
        public int[] api_nowhps;
        public int[] api_maxhps;
        #endregion

        public int[] api_f_nowhps;
        public int[] api_f_maxhps;
        public int[] api_e_nowhps;
        public int[] api_e_maxhps;
        public int[][] api_eSlot;

        public BattleDetailJson.Aerial api_air_base_attack;
        public int api_lost_kind;
    }

    internal class HPNaConverter : JsonConverter<int[]>
    {
        public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer) => throw new NotImplementedException();
        public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray values = serializer.Deserialize<JArray>(reader);
            return values.Select(x => x.Type is JTokenType.Integer ? ((int)x) : 1).ToArray();
        }
    }
}
