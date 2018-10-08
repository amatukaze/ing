using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Expedition")]
    public sealed partial class ExpeditionView : UserControl
    {
        private readonly NavalBase ViewModel;
        public ExpeditionView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
