using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Node
    {
        [JsonProperty("id")]
        public int ID { get; internal set; }

        [JsonProperty("x")]
        public int PositionX { get; internal set; }
        [JsonProperty("y")]
        public int PositionY { get; internal set; }

        [JsonProperty("wiki_id")]
        public string WikiID { get; internal set; }

        internal Node() { }
    }
}
