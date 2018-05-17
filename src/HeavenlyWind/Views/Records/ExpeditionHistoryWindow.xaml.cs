using Sakuno.KanColle.Amatsukaze.ViewModels.Records;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Records
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

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
