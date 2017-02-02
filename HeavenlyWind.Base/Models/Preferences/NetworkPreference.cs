namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class NetworkPreference : ModelBase
    {
        public Property<bool> PortCustomization { get; } = new Property<bool>("network.port.customization", false);
        public Property<int> Port { get; } = new Property<int>("network.port", 15820);

        public Property<bool> AllowRequestsFromOtherDevices { get; } = new Property<bool>("network.allow_remote_requests");

        public UpstreamProxyPreference UpstreamProxy { get; } = new UpstreamProxyPreference();

        public Property<bool> AutoRetry { get; } = new Property<bool>("network.auto_retry.enabled");
        public Property<int> AutoRetryCount { get; } = new Property<int>("network.auto_retry.count", 3);
        public Property<bool> AutoRetryConfirmation { get; } = new Property<bool>("network.auto_retry.confirmation", true);
    }
}
