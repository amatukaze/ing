using Sakuno.KanColle.Amatsukaze.ViewModels.Statistics;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Statistics
{
    /// <summary>
    /// SortieStatisticWindow.xaml の相互作用ロジック
    /// </summary>
    partial class SortieStatisticWindow
    {
        SortieStatisticViewModel r_ViewModel;

        public SortieStatisticWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new SortieStatisticViewModel();

            Loaded += (s, e) => r_ViewModel.Load();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
