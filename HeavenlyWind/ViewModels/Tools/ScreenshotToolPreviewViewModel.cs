using Sakuno.KanColle.Amatsukaze.Services;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class ScreenshotToolPreviewViewModel : ModelBase
    {
        OverviewScreenshotToolViewModel r_Owner;

        public BitmapSource Screenshot { get; private set; }

        public ICommand TakeScreenshotCommand { get; private set; }

        public ScreenshotToolPreviewViewModel(OverviewScreenshotToolViewModel rpOwner)
        {
            r_Owner = rpOwner;

            TakeScreenshotCommand = new DelegatedCommand(async () =>
            {
                Screenshot = await ScreenshotService.Instance.TakePartialScreenshot(ScreenshotToolViewModel.Regions[r_Owner.Type]);
                OnPropertyChanged(nameof(Screenshot));
            });
        }
    }
}
