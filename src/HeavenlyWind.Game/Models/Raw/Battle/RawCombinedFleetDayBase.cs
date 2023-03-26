using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public abstract class RawCombinedFleetDayBase : RawDay, IRawCombinedFleet
    {
        [JsonProperty("api_f_nowhps_combined")]
        public int[] FriendEscortCurrentHPs { get; set; }
        [JsonProperty("api_f_maxhps_combined")]
        public int[] FriendEscortMaximumHPs { get; set; }

        [JsonProperty("api_e_nowhps_combined")]
        public JArray RawEnemyEscortCurrentHPs { get; set; }
        [JsonProperty("api_e_maxhps_combined")]
        public JArray RawEnemyEscortMaximumHPs { get; set; }

        public int[] EnemyEscortCurrentHPs
        {
            get
            {
                if (RawEnemyEscortCurrentHPs is null) return null;

                var result = new int[RawEnemyEscortCurrentHPs.Count];

                for (int i = 0; i < result.Length; i++)
                {
                    var item = RawEnemyEscortCurrentHPs[i];

                    if (item.Type is JTokenType.Integer)
                    {
                        result[i] = item.ToObject<int>();
                        continue;
                    }

                    result[i] = -99999;
                }

                return result;
            }
        }
        public int[] EnemyEscortMaximumHPs
        {
            get
            {
                if (RawEnemyEscortMaximumHPs is null) return null;

                var result = new int[RawEnemyEscortMaximumHPs.Count];

                for (int i = 0; i < result.Length; i++)
                {
                    var item = RawEnemyEscortMaximumHPs[i];

                    if (item.Type is JTokenType.Integer)
                    {
                        result[i] = item.ToObject<int>();
                        continue;
                    }

                    result[i] = -99999;
                }

                return result;
            }
        }

        [JsonProperty("api_hougeki3")]
        public RawShellingPhase ShellingThirdRound { get; set; }
    }
}
