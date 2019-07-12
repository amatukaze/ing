using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Combat
{
    [ExportView("MapHP")]
    public sealed partial class MapHPView : UserControl
    {
        private readonly NavalBase ViewModel;

        public MapHPView(NavalBase navalBase)
        {
            ViewModel = navalBase;
            InitializeComponent();
        }

        public static string SelectRankText(EventMapRank? rank)
            => rank switch
            {
                EventMapRank.Hard => "甲",
                EventMapRank.Normal => "乙",
                EventMapRank.Easy => "丙",
                EventMapRank.Casual => "丁",
                _ => string.Empty
            };

        public static string SelectRankTooltip(EventMapRank? rank)
            => rank switch
            {
                EventMapRank.Hard => "甲 Hard",
                EventMapRank.Normal => "乙 Normal",
                EventMapRank.Easy => "丙 Easy",
                EventMapRank.Casual => "丁 Casual",
                _ => null
            };
    }
}
