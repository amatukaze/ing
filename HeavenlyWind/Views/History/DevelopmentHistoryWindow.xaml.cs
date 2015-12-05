using System;
using Sakuno.KanColle.Amatsukaze.ViewModels.History;

namespace Sakuno.KanColle.Amatsukaze.Views.History
{
    /// <summary>
    /// DevelopmentHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class DevelopmentHistoryWindow
    {
        DevelopmentHistoryViewModel r_ViewModel;

        public DevelopmentHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new DevelopmentHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
