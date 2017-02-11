namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class PanicKeyPreference : ModelBase
    {
        public Property<bool> Enabled { get; } = new Property<bool>("other.panic_key.enabled");

        public Property<int> ModifierKeys { get; } = new Property<int>("other.panic_key.modifier_keys");

        public Property<int> Key { get; } = new Property<int>("other.panic_key.key");

        public void UpdateKey(int rpModiferKeys, int rpKey)
        {
            ModifierKeys.Value = rpModiferKeys;
            Key.SetDirectly(rpKey);

            Key.NotifyUpdate();
        }
    }
}
