using Sakuno.KanColle.Amatsukaze.ViewModels.Records;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Records
{
    /// <summary>
    /// ResourceHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ResourceHistoryWindow
    {
        ResourceHistoryViewModel r_ViewModel;

        public ResourceHistoryWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new ResourceHistoryViewModel();

            Loaded += (s, e) => r_ViewModel.LoadRecords();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
