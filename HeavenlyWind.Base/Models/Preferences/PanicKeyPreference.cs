using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class PanicKeyPreference : ModelBase
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

        [JsonProperty("modifier_keys")]
        public int ModifierKeys { get; set; }

        [JsonProperty("key")]
        public int Key { get; set; }

        public void UpdateKey(int rpModiferKeys, int rpKey)
        {
            ModifierKeys = rpModiferKeys;
            Key = rpKey;

            OnPropertyChanged(nameof(Key));
        }
    }
}
