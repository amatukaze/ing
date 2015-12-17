using Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Ships;

namespace Sakuno.KanColle.Amatsukaze.Views.Overviews
{
    /// <summary>
    /// ShipOverViewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ShipOverviewWindow
    {
        public ShipOverviewWindow()
        {
            InitializeComponent();

            DataContext = new ShipOverviewViewModel();
        }
    }
}
