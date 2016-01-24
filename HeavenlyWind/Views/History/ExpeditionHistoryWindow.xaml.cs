using Sakuno.KanColle.Amatsukaze.ViewModels.History;

namespace Sakuno.KanColle.Amatsukaze.Views.History
{
    /// <summary>
    /// ExpeditionHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ExpeditionHistoryWindow
    {
        ExpeditionHistoryViewModel r_ViewModel;

        public ExpeditionHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new ExpeditionHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }
    }
}
