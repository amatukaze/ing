using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UpstreamProxyPreference
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty("host")]
        public string Host { get; set; } = "127.0.0.1";

        [JsonProperty("port")]
        public int Port { get; set; } = 15820;

        [JsonProperty("usessl")]
        public bool UseSSL { get; set; } = false;
    }
}
