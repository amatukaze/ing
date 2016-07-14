using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference
    {
        [JsonProperty("port")]
        public Property<int> Port { get; private set; } = new Property<int>(15820);

        [JsonProperty("enableforssl")]
        public Property<bool> EnableForSSL { get; private set; } = new Property<bool>();

        [JsonProperty("allowremoterequests")]
        public Property<bool> AllowRequestsFromOtherDevices { get; private set; } = new Property<bool>();

        [JsonProperty("upstreamproxy")]
        public UpstreamProxyPreference UpstreamProxy { get; private set; } = new UpstreamProxyPreference();
    }
}
