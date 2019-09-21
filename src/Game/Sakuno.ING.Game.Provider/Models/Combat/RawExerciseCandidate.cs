using Newtonsoft.Json;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawExerciseCandidate
    {
        [JsonProperty("api_member_id")]
        public int AdmiralId { get; internal set; }
        [JsonProperty("api_nickname")]
        public string Name { get; internal set; }
        [JsonProperty("api_cmt")]
        public string Comment { get; internal set; }
        [JsonProperty("api_rank")]
        public AdmiralRank Rank { get; internal set; }

        internal int api_level;
        internal int[] api_experience;
        public Leveling Leveling => new Leveling(api_level,
            api_experience.At(0),
            KnownLeveling.GetAdmiralExp(api_level),
            KnownLeveling.GetAdmiralExp(api_level + 1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        internal int[] api_ship;
        public int MaxShipCount => api_ship.At(1);

        internal int[] api_slotitem;
        public int MaxEquipmentCount => api_slotitem.At(1);

        [JsonProperty("api_furniture")]
        public int FurnitureCount { get; internal set; }
    }
}
