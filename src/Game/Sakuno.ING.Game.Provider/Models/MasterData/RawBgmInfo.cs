using Newtonsoft.Json;

namespace Sakuno.ING.Game.Models.MasterData
{
    public sealed class RawBgmInfo : IIdentifiable
    {
        internal RawBgmInfo() { }

        [JsonProperty("api_id")]
        public int Id { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }
    }
}
