using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference
    {
        [JsonProperty("port")]
        public int Port { get; set; } = 15820;
        
        [JsonProperty("usessl")]
        public bool UseSSL { get; set; } = false;

        [JsonProperty("upstreamproxy")]
        public UpstreamProxyPreference UpstreamProxy { get; set; } = new UpstreamProxyPreference();
    }
}
