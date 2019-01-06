using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawUseItemCount : IIdentifiable<UseItemId>
    {
        internal RawUseItemCount() { }

        [JsonProperty("api_id")]
        public UseItemId Id { get; internal set; }
        [JsonProperty("api_count")]
        public int Count { get; internal set; }
    }
}
