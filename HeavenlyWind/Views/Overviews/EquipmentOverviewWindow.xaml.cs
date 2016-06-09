using Sakuno.KanColle.Amatsukaze.ViewModels.Overviews;
using System;

namespace Sakuno.KanColle.Amatsukaze.Views.Overviews
{
    /// <summary>
    /// EquipmentOverviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EquipmentOverviewWindow
    {
        EquipmentOverviewViewModel r_ViewModel;

        public EquipmentOverviewWindow()
        {
            InitializeComponent();

            DataContext = r_ViewModel = new EquipmentOverviewViewModel();
        }

        protected override void OnClosed(EventArgs e)
        {
            r_ViewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}
