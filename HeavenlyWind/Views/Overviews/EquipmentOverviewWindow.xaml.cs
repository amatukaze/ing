using Sakuno.KanColle.Amatsukaze.ViewModels.Overviews;

namespace Sakuno.KanColle.Amatsukaze.Views.Overviews
{
    /// <summary>
    /// EquipmentOverviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EquipmentOverviewWindow
    {
        public EquipmentOverviewWindow()
        {
            InitializeComponent();

            DataContext = new EquipmentOverviewViewModel();
        }
    }
}
