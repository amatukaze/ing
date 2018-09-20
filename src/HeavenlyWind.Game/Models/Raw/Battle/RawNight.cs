using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawNight : RawBattleBase
    {
        [JsonProperty("api_deck_id")]
        public override int FleetID { get; set; }

        [JsonProperty("api_friendly_battle")]
        public RawNpcSupportingFire NpcSupportingFire { get; set; }

        //[JsonProperty("api_touch_plane")]

        //[JsonProperty("api_flare_pos")]

        [JsonProperty("api_hougeki")]
        public RawShellingPhase Shelling { get; set; }

        public class RawNpcSupportingFire
        {
            [JsonProperty("api_hougeki")]
            public RawShellingPhase Shelling { get; set; }
        }
    }
}
