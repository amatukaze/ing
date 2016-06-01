using Sakuno.KanColle.Amatsukaze.ViewModels.History;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.History
{
    /// <summary>
    /// ScrappingHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ScrappingHistoryWindow
    {
        ScrappingHistoryViewModel r_ViewModel;

        public ScrappingHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new ScrappingHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
