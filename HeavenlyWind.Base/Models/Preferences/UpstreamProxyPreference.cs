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
        public int Port { get; set; } = 0;

        [JsonProperty("usessl")]
        public bool UseSSL { get; set; } = false;

        string r_Address;
        [JsonIgnore]
        public string Address
        {
            get { return r_Address ?? (Address = $"{Host}:{Port}"); }
            set { r_Address = value; }
        }
    }
}
