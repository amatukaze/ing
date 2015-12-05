using Sakuno.KanColle.Amatsukaze.ViewModels.History;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.History
{
    /// <summary>
    /// ConstructionHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ConstructionHistoryWindow
    {
        ConstructionHistoryViewModel r_ViewModel;

        public ConstructionHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new ConstructionHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
