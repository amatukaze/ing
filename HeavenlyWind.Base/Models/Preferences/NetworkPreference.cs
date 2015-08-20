using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference
    {
        [JsonProperty("port")]
        public int Port { get; set; } = 15789;

        [JsonProperty("upstreamproxy")]
        public UpstreamProxyPreference UpstreamProxy { get; set; } = new UpstreamProxyPreference();
    }
}
