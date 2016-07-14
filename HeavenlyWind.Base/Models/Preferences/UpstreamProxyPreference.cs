using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UpstreamProxyPreference
    {
        [JsonProperty("enabled")]
        public Property<bool> Enabled { get; private set; } = new Property<bool>();

        [JsonProperty("host")]
        public Property<string> Host { get; private set; } = new Property<string>("127.0.0.1");

        [JsonProperty("port")]
        public Property<int> Port { get; private set; } = new Property<int>();

        [JsonProperty("http_only")]
        public Property<bool> HttpOnly { get; private set; } = new Property<bool>();
    }
}
