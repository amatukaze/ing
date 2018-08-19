using Sakuno.SystemInterop;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    public static class CommunicatorMessages
    {
        public const string Ready = nameof(Ready);
        public const string Initialize = nameof(Initialize);
        public const string InitializeBlink = nameof(InitializeBlink);
        public const string SetPort = nameof(SetPort);
        public const string Attach = nameof(Attach);

        public const string ClearCache = nameof(ClearCache);
        public const string ClearCookie = nameof(ClearCookie);

        public const string GoBack = nameof(GoBack);
        public const string GoForward = nameof(GoForward);
        public const string Navigate = nameof(Navigate);
        public const string Refresh = nameof(Refresh);

        public const string LoadCompleted = nameof(LoadCompleted);
        public const string LoadGamePageCompleted = nameof(LoadGamePageCompleted);

        public const string SetZoom = nameof(SetZoom);
        public const string InvalidateArrange = nameof(InvalidateArrange);

        public const string ResizeBrowserToFitGame = nameof(ResizeBrowserToFitGame);

        public const string SetBlinkMaxFramerate = nameof(SetBlinkMaxFramerate);

        public const string TakeScreenshot = nameof(TakeScreenshot);
        public const string ScreenshotFail = nameof(ScreenshotFail);
        public const string StartScreenshotTransmission = nameof(StartScreenshotTransmission);
        public const string FinishScreenshotTransmission = nameof(FinishScreenshotTransmission);

        public static readonly NativeConstants.WindowMessage ResizeBrowserWindow;

        static CommunicatorMessages()
        {
            var rAppMessage = (int)NativeConstants.WindowMessage.WM_APP;
            ResizeBrowserWindow = (NativeConstants.WindowMessage)(rAppMessage + 1);
        }

    }
}
