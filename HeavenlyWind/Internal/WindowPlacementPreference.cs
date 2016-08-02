using Sakuno.UserInterface.Controls;
using Sakuno.SystemInterop;
using System.Windows;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class WindowPlacementPreference : IWindowPlacementPreference
    {
        public NativeStructs.WINDOWPLACEMENT? Load(Window rpWindow)
        {
            var rPlacement = Preference.Instance.Windows.LoadPlacement(rpWindow.GetType().FullName);
            if (rPlacement == null)
                return null;

            var rResult = new NativeStructs.WINDOWPLACEMENT()
            {
                rcNormalPosition = new NativeStructs.RECT(rPlacement.Left, rPlacement.Top, rPlacement.Left+rPlacement.Width,rPlacement.Top+rPlacement.Height),
            };

            switch (rPlacement.State)
            {
                case WindowState.Normal:
                    rResult.showCmd = NativeConstants.ShowCommand.SW_SHOWNORMAL;
                    break;

                case WindowState.Minimized:
                    rResult.showCmd = NativeConstants.ShowCommand.SW_SHOWMINIMIZED;
                    break;

                case WindowState.Maximized:
                    rResult.showCmd = NativeConstants.ShowCommand.SW_SHOWMAXIMIZED;
                    break;
            }

            return rResult;
        }

        public void Save(Window rpWindow, NativeStructs.WINDOWPLACEMENT rpData)
        {
            var rPreference = new WindowPreference()
            {
                Name = rpWindow.GetType().FullName,

                Left = rpData.rcNormalPosition.Left,
                Top = rpData.rcNormalPosition.Top,
                Width = rpData.rcNormalPosition.Right - rpData.rcNormalPosition.Left,
                Height = rpData.rcNormalPosition.Bottom - rpData.rcNormalPosition.Top,
                State = rpWindow.WindowState,
            };

            Preference.Instance.Windows.SavePlacement(rPreference);
        }
    }
}
