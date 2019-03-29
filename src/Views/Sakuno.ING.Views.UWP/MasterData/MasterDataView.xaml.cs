using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.MasterData
{
    [ExportView("MasterData")]
    public sealed partial class MasterDataView : UserControl
    {
        private readonly MasterDataRoot MasterData;
        public MasterDataView(NavalBase navalBase)
        {
            MasterData = navalBase.MasterData;
            this.InitializeComponent();
        }
    }
}
