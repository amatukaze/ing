using System.Windows.Controls;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;

namespace Sakuno.ING.Views.Desktop.Logging
{
    [ExportView("BattleLogs")]
    public partial class BattleLogsView : UserControl
    {
        public BattleLogsView()
        {
            InitializeComponent();
            Unloaded += (s, e) => (DataContext as BattleLogsVM)?.Dispose();
        }
    }
}
