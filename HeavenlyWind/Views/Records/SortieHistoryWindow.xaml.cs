using Sakuno.KanColle.Amatsukaze.ViewModels.Records;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Records
{
    /// <summary>
    /// SortieHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class SortieHistoryWindow
    {
        SortieHistoryViewModel r_ViewModel;

        public SortieHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new SortieHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
