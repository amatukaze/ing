using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference
    {
        [JsonProperty("port")]
        public int Port { get; set; } = 15820;
        
        [JsonProperty("enableforssl")]
        public bool EnableForSSL { get; set; } = false;

        [JsonProperty("upstreamproxy")]
        public UpstreamProxyPreference UpstreamProxy { get; set; } = new UpstreamProxyPreference();
    }
}
