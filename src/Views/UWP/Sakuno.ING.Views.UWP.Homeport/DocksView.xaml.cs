using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Homeport
{
    [ExportView("Docks")]
    public sealed partial class DocksView : UserControl
    {
        private readonly NavalBase ViewModel;
        public DocksView(NavalBase viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
