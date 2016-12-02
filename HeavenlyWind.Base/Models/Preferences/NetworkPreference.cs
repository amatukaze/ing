namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference
    {
        public Property<bool> PortCustomization { get; } = new Property<bool>("network.port.customization", false);
        public Property<int> Port { get; } = new Property<int>("network.port", 15820);

        public Property<bool> AllowRequestsFromOtherDevices { get; } = new Property<bool>("network.allow_remote_requests");

        public UpstreamProxyPreference UpstreamProxy { get; } = new UpstreamProxyPreference();
    }
}
