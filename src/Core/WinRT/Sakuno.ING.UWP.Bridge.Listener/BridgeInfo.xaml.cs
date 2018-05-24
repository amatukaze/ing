using Sakuno.ING.Composition;
using Sakuno.ING.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Sakuno.ING.UWP.Bridge
{
    public sealed partial class BridgeInfo : UserControl
    {
        private string Version = Constants.Version;
        private Provider Provider = (Provider)StaticResolver.Instance.Resolve<ITextStreamProvider>();
        public BridgeInfo()
        {
            this.InitializeComponent();
        }

        private Visibility Not(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
    }
}
