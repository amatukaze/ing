using System.Linq;
using Sakuno.ING.Browser.Desktop;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IHttpProviderSelector))]
    [Export(typeof(BrowserSelector))]
    internal class BrowserSelector : IHttpProviderSelector
    {
        private readonly ISettingItem<string> browserSetting;
        public BrowserSelector(IBrowserProvider[] browsers, ISettingsManager settings)
        {
            browserSetting = settings.Register<string>("browser_engine");
            string engine = browserSetting.InitialValue;
            SelectedBrowser = browsers.FirstOrDefault(x => x.Id == engine)
                ?? browsers.FirstOrDefault();
            SelectedBrowser.Initialize();
        }

        public IBrowserProvider SelectedBrowser { get; }
        IHttpProvider IHttpProviderSelector.Current => SelectedBrowser.HttpProvider;
    }
}
