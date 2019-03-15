using System;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [Export(typeof(IHttpProviderSelector))]
    [Export(typeof(BrowserSelector))]
    internal class BrowserSelector : IHttpProviderSelector
    {
        public IBindableCollection<string> AvailableBrowsers { get; }
        public BrowserSetting Settings { get; }
        public BrowserSelector(IBrowserProvider[] browsers, BrowserSetting settings, IShellContextService shell)
        {
            browsers = browsers.Where(x => x.IsSupported).ToArray();
            AvailableBrowsers = browsers.Select(x => x.Id).ToBindable();

            Settings = settings;
            if (settings.Debug.Value)
                Current = new DebugHttpProvider(shell);
            else
            {
                string engine = settings.BrowserEngine.InitialValue;
                SelectedBrowser = browsers.FirstOrDefault(x => x.Id == engine)
                    ?? browsers.FirstOrDefault()
                    ?? throw new InvalidOperationException("It must have at least one browser provider.");
                SelectedBrowser.Initialize();
                Current = SelectedBrowser.HttpProvider;
            }
        }

        public IBrowserProvider SelectedBrowser { get; }
        public IHttpProvider Current { get; }
    }
}
