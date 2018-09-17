using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Timing;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IHttpProviderSelector))]
    internal class UWPHttpProviderSelector : IHttpProviderSelector
    {
        public UWPHttpProviderSelector(ITimingService dateTime, IHttpProxy proxy)
        {
            Current = proxy;
        }
        public IHttpProvider Current { get; }
    }
}
