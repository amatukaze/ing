using Sakuno.ING.Composition;

namespace Sakuno.ING.Settings
{
    [Export(typeof(ProxySetting))]
    public class ProxySetting
    {
        public ProxySetting(ISettingsManager manager)
        {
            ListeningPort = manager.Register("proxy.listening_port", 15551);
            UseUpstream = manager.Register("proxy.upstream.enabled", false);
            Upstream = manager.Register("proxy.upstream", "localhost");
            UpstreamPort = manager.Register("proxy.upstream.port", 8099);
        }

        public ISettingItem<int> ListeningPort { get; }
        public ISettingItem<bool> UseUpstream { get; }
        public ISettingItem<string> Upstream { get; }
        public ISettingItem<int> UpstreamPort { get; }
    }
}
