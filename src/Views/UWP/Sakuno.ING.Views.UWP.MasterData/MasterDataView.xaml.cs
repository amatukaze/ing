using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.MasterData
{
    [ExportView("MasterData")]
    public sealed partial class MasterDataView : UserControl
    {
        private readonly MasterDataRoot MasterData;
        public MasterDataView(MasterDataRoot masterData)
        {
            MasterData = masterData;
            this.InitializeComponent();
        }
    }
}
