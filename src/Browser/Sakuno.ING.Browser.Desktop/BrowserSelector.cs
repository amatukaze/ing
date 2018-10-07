using System;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Browser.Desktop
{
    [Export(typeof(IHttpProviderSelector))]
    [Export(typeof(BrowserSelector))]
    internal class BrowserSelector : IHttpProviderSelector
    {
        public ISettingItem<string> BrowserEngine { get; }
        public ISettingItem<string> DefaultUrl { get; }
        public IBindableCollection<string> AvailableBrowsers { get; }
        public BrowserSelector(IBrowserProvider[] browsers, ISettingsManager settings)
        {
            browsers = browsers.Where(x => x.IsSupported).ToArray();
            AvailableBrowsers = browsers.Select(x => x.Id).ToBindable();

            BrowserEngine = settings.Register<string>("browser_engine");
            DefaultUrl = settings.Register("browser_gameurl", "http://www.dmm.com/netgame_s/kancolle/");

            string engine = BrowserEngine.InitialValue;
            SelectedBrowser = browsers.FirstOrDefault(x => x.Id == engine)
                ?? browsers.FirstOrDefault()
                ?? throw new InvalidOperationException("It must have at least one browser provider.");
            SelectedBrowser.Initialize();
        }

        public IBrowserProvider SelectedBrowser { get; }
        IHttpProvider IHttpProviderSelector.Current => SelectedBrowser.HttpProvider;
    }
}
