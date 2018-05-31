using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.MasterData
{
    public sealed partial class MasterDataView : UserControl
    {
        private readonly MasterDataRoot MasterData = StaticResolver.Instance.Resolve<NavalBase>().MasterData;
        public MasterDataView()
        {
            this.InitializeComponent();
        }
    }
}
