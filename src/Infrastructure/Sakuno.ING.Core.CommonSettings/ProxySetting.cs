namespace Sakuno.ING.Settings
{
    public class ProxySetting
    {
        public ProxySetting(ISettingsManager manager)
        {
            ListeningPort = manager.Register("proxy_listening_port", 15551);
            UseUpstream = manager.Register("proxy_upstream_enabled", false);
            Upstream = manager.Register("proxy_upstream", "localhost");
            UpstreamPort = manager.Register("proxy_upstream_port", 8099);
        }

        public ISettingItem<int> ListeningPort { get; }
        public ISettingItem<bool> UseUpstream { get; }
        public ISettingItem<string> Upstream { get; }
        public ISettingItem<int> UpstreamPort { get; }
    }
}
