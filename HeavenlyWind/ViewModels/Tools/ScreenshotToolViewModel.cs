using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class ScreenshotToolViewModel : ModelBase
    {
        internal static readonly Dictionary<ScreenshotRegion, Int32Rect> Regions = new Dictionary<ScreenshotRegion, Int32Rect>()
        {
            { ScreenshotRegion.All, new Int32Rect(0, 0, 800, 480) },
            { ScreenshotRegion.FleetComposition, new Int32Rect(105, 100, 690, 370) },
            { ScreenshotRegion.Materials, new Int32Rect(651, 6, 149, 64) },
            { ScreenshotRegion.ShipDetail, new Int32Rect(319, 102, 468, 366) },
            { ScreenshotRegion.ShipList, new Int32Rect(356, 100, 441, 362) },
        };

        public OverviewScreenshotToolViewModel Overview { get; }

        bool r_OutputToClipboard;
        public bool OutputToClipboard
        {
            get { return r_OutputToClipboard; }
            internal set
            {
                if (r_OutputToClipboard != value)
                {
                    r_OutputToClipboard = value;
                    OnPropertyChanged(nameof(OutputToClipboard));
                }
            }
        }

        public ICommand TakeGeneralScreenshotCommand { get; }

        public ScreenshotToolViewModel()
        {
            Overview = new OverviewScreenshotToolViewModel(this);

            TakeGeneralScreenshotCommand = new DelegatedCommand<ScreenshotRegion>(r => ScreenshotService.Instance.TakePartialScreenshotAndOutput(Regions[r], r_OutputToClipboard));
        }
    }
}
