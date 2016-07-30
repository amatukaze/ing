namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UpstreamProxyPreference
    {
        public Property<bool> Enabled { get; } = new Property<bool>("network.upstream_proxy.enabled");

        public Property<string> Host { get; } = new Property<string>("network.upstream_proxy.host", "127.0.0.1");

        public Property<int> Port { get; } = new Property<int>("network.upstream_proxy.port");

        public Property<bool> HttpOnly { get; } = new Property<bool>("network.upstream_proxy.http_only");
    }
}
