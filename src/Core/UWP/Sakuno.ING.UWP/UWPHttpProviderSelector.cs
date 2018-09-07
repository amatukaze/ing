using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Services.Listener;
using Sakuno.ING.Settings;
using Sakuno.ING.Timing;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IHttpProviderSelector))]
    internal class UWPHttpProviderSelector : IHttpProviderSelector
    {
        public UWPHttpProviderSelector(ITimingService dateTime, ProxySetting setting)
        {
            Current = new NekomimiProvider(dateTime, setting);
        }
        public IHttpProvider Current { get; }
    }
}
