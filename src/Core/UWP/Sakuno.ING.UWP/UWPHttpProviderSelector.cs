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
        public UWPHttpProviderSelector(IShellContextService shell, ISettingsManager settings)
        {
            Debug = settings.Register("browser.debug_data", false);
            DefaultUrl = settings.Register("browser.game_url", "http://www.dmm.com/netgame_s/kancolle/");

            if (Debug.InitialValue)
                Current = new DebugHttpProvider(shell);
            else
                Current = new DebugHttpProvider(shell);
        }

        public ISettingItem<bool> Debug { get; }
        public ISettingItem<string> DefaultUrl { get; }
        public IHttpProvider Current { get; }
    }
}
