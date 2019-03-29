using System.Windows.Controls;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;

namespace Sakuno.ING.Views.Desktop.Logging
{
    [ExportView("BattleLogDetail")]
    public partial class BattleLogDetailsView : UserControl
    {
        public BattleLogDetailsView(BattleVM viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
