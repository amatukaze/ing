using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UpstreamProxyPreference : ModelBase
    {
        bool r_Enabled;
        [JsonProperty("enabled")]
        public bool Enabled
        {
            get { return r_Enabled; }
            set
            {
                if (r_Enabled != value)
                {
                    r_Enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        string r_Host = "127.0.0.1";
        [JsonProperty("host")]
        public string Host
        {
            get { return r_Host; }
            set
            {
                if (r_Host != value)
                {
                    r_Host = value;
                    OnPropertyChanged(nameof(Host));
                }
            }
        }

        int r_Port;
        [JsonProperty("port")]
        public int Port
        {
            get { return r_Port; }
            set
            {
                if (r_Port != value)
                {
                    r_Port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        string r_Address;
        [JsonIgnore]
        public string Address
        {
            get { return r_Address ?? (Address = $"{Host}:{Port}"); }
            set { r_Address = value; }
        }

        [JsonProperty("http_only")]
        bool r_HttpOnly;
        public bool HttpOnly
        {
            get { return r_HttpOnly; }
            set
            {
                if (r_HttpOnly != value)
                {
                    r_HttpOnly = value;
                    OnPropertyChanged(nameof(HttpOnly));
                }
            }
        }
    }
}
