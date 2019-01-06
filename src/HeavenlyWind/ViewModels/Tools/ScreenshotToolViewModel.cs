using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.Tools;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System.Windows;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class ScreenshotToolViewModel : ModelBase
    {
        internal static readonly ListDictionary<ScreenshotRegion, Int32Rect> Regions = new ListDictionary<ScreenshotRegion, Int32Rect>()
        {
            { ScreenshotRegion.All, new Int32Rect(0, 0, 1200, 720) },
            { ScreenshotRegion.FleetComposition, new Int32Rect(158, 150, 1035, 555) },
            { ScreenshotRegion.Materials, new Int32Rect(976, 9, 224, 96) },
            { ScreenshotRegion.ShipDetail, new Int32Rect(480, 153, 702, 549) },
            { ScreenshotRegion.ShipList, new Int32Rect(534, 150, 662, 543) },
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

        ScreenshotToolOverlayWindow r_OverlayWindow;
        public ICommand ShowOverlayWindowCommand { get; }
        public ICommand HideOverlayWindowCommand { get; }

        public ScreenshotToolViewModel()
        {
            Overview = new OverviewScreenshotToolViewModel(this);

            TakeGeneralScreenshotCommand = new DelegatedCommand<ScreenshotRegion>(r => ScreenshotService.Instance.TakePartialScreenshotAndOutput(Regions[r], r_OutputToClipboard));

            ShowOverlayWindowCommand = new DelegatedCommand<ScreenshotRegion>(r =>
            {
                r_OverlayWindow = new ScreenshotToolOverlayWindow();
                r_OverlayWindow.Show(Regions[r]);
            });
            HideOverlayWindowCommand = new DelegatedCommand(() =>
            {
                r_OverlayWindow?.Hide();
                r_OverlayWindow = null;
            });
        }
    }
}
