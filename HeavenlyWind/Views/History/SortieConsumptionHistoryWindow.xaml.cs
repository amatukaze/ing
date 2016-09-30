using Sakuno.KanColle.Amatsukaze.ViewModels.History;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.History
{
    /// <summary>
    /// SortieConsumptionHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class SortieConsumptionHistoryWindow
    {
        SortieConsumptionHistoryViewModel r_ViewModel;

        public SortieConsumptionHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new SortieConsumptionHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
