using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.UWP
{
    [Export(typeof(UWPHttpProviderSelector))]
    [Export(typeof(IHttpProviderSelector))]
    internal class UWPHttpProviderSelector : IHttpProviderSelector
    {
        public UWPHttpProviderSelector(IShellContextService shell, BrowserSetting settings, ProxySetting proxy)
        {
            if (settings.Debug.InitialValue)
                Current = new DebugHttpProvider(shell);
            else
                Current = new EdgeHttpProvider(proxy);
            Settings = settings;
        }

        public IHttpProvider Current { get; }
        public BrowserSetting Settings { get; }
    }
}
