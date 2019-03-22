using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Fleets")]
    public sealed partial class FleetsView : UserControl
    {
        private readonly NavalBase ViewModel;
        public FleetsView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
