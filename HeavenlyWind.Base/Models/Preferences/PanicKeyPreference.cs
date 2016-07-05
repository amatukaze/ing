using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class PanicKeyPreference
    {
        [JsonProperty("enabled")]
        public Property<bool> Enabled { get; private set; } = new Property<bool>();

        [JsonProperty("modifier_keys")]
        public Property<int> ModifierKeys { get; private set; } = new Property<int>();

        [JsonProperty("key")]
        public Property<int> Key { get; private set; } = new Property<int>();

        public void UpdateKey(int rpModiferKeys, int rpKey)
        {
            ModifierKeys.Value = rpModiferKeys;
            Key.SetDirectly(rpKey);

            Key.NotifyUpdate();
        }
    }
}
