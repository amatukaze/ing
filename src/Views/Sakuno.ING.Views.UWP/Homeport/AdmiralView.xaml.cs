using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Admiral")]
    public sealed partial class AdmiralView : UserControl
    {
        private readonly NavalBase ViewModel;
        public AdmiralView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
