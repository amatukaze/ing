using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.SystemInterop;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class ScreenshotService
    {
        public static ScreenshotService Instance { get; } = new ScreenshotService();

        TaskCompletionSource<BitmapSource> r_TaskScreenshotTask;

        ScreenshotService()
        {
            var rMessages = BrowserService.Instance.Communicator.GetMessageObservable();
            rMessages.Subscribe(CommunicatorMessages.ScreenshotFail, _ => ScreenshotFail());
            rMessages.Subscribe(CommunicatorMessages.StartScreenshotTransmission, rpParameter =>
            {
                var rParameters = rpParameter.Split(';');

                var rMapName = rParameters[0];
                var rWidth = int.Parse(rParameters[1]);
                var rHeight = int.Parse(rParameters[2]);
                GetScreenshot(rMapName, rWidth, rHeight);
            });
        }

        public async Task<BitmapSource> TakeScreenshot(Func<BitmapSource, BitmapSource> rpProcessAction = null)
        {
            r_TaskScreenshotTask = new TaskCompletionSource<BitmapSource>();
            BrowserService.Instance.Communicator.Write(CommunicatorMessages.TakeScreenshot);

            var rImage = await r_TaskScreenshotTask.Task;
            
            if (rpProcessAction != null)
                rImage = rpProcessAction(rImage);

            r_TaskScreenshotTask = null;

            return rImage;
        }
        public Task<BitmapSource> TakePartialScreenshot(Int32Rect rpRect)
        {
            return TakeScreenshot(r =>
            {
                var rResult = new CroppedBitmap(r, rpRect);
                rResult.Freeze();

                return rResult;
            });
        }
        public async void TakeScreenshotAndOutput(Func<BitmapSource, BitmapSource> rpProcessAction = null, bool rpOutputToClipboard = true)
        {
            if (r_TaskScreenshotTask != null)
                return;

            var rImage = await TakeScreenshot(rpProcessAction);
            if (rImage == null)
                return;

            if (rpOutputToClipboard)
                OutputToClipboard(rImage);
            else
                OutputAsFile(rImage);
        }
        public async void TakePartialScreenshotAndOutput(Int32Rect rpRect, bool rpOutputToClipboard)
        {
            if (r_TaskScreenshotTask != null)
                return;

            var rImage = await TakePartialScreenshot(rpRect);
            if (rImage == null)
                return;

            if (rpOutputToClipboard)
                OutputToClipboard(rImage);
            else
                OutputAsFile(rImage);
        }

        void ScreenshotFail()
        {
            r_TaskScreenshotTask.SetResult(null);
        }

        public void OutputToClipboard(BitmapSource rpImage)
        {
            Clipboard.SetImage(rpImage);
        }
        public void OutputAsFile(BitmapSource rpImage)
        {
            var rPreference = Preference.Current.Browser.Screenshot;

            string rExtension;
            BitmapEncoder rEncoder;
            switch (rPreference.ImageFormat)
            {
                case ScreenshotImageFormat.Png:
                    rExtension = "png";
                    rEncoder = new PngBitmapEncoder();
                    break;

                case ScreenshotImageFormat.Jpeg:
                    rExtension = "jpg";
                    rEncoder = new JpegBitmapEncoder();
                    break;

                case ScreenshotImageFormat.Bmp:
                    rExtension = "bmp";
                    rEncoder = new BmpBitmapEncoder();
                    break;

                default: throw new InvalidEnumArgumentException(nameof(rPreference.ImageFormat), (int)rPreference.ImageFormat, typeof(ScreenshotImageFormat));
            }

            var rPath = Path.Combine(rPreference.Destination, string.Format(rPreference.FilenameFormat, DateTime.Now, rExtension));
            var rDirectory = Path.GetDirectoryName(rPath);
            if (!Directory.Exists(rDirectory))
                Directory.CreateDirectory(rDirectory);

            using (var rFile = File.Open(rPath, FileMode.Create))
            {
                rEncoder.Frames.Add(BitmapFrame.Create(rpImage));
                rEncoder.Save(rFile);
            }
        }

        void GetScreenshot(string rpMapName, int rpWidth, int rpHeight)
        {
            using (var rMap = MemoryMappedFile.CreateOrOpen(rpMapName, rpWidth * rpHeight * 3, MemoryMappedFileAccess.ReadWrite))
            {
                var rInfo = new NativeStructs.BITMAPINFO();
                rInfo.bmiHeader.biSize = Marshal.SizeOf(typeof(NativeStructs.BITMAPINFOHEADER));
                rInfo.bmiHeader.biWidth = rpWidth;
                rInfo.bmiHeader.biHeight = rpHeight;
                rInfo.bmiHeader.biBitCount = 24;
                rInfo.bmiHeader.biPlanes = 1;

                IntPtr rBits;
                var rHBitmap = NativeMethods.Gdi32.CreateDIBSection(IntPtr.Zero, ref rInfo, 0, out rBits, rMap.SafeMemoryMappedFileHandle.DangerousGetHandle(), 0);
                if (rHBitmap == IntPtr.Zero)
                    throw new InvalidOperationException();

                var rImage = Imaging.CreateBitmapSourceFromHBitmap(rHBitmap, IntPtr.Zero, new Int32Rect(0, 0, rpWidth, rpHeight), BitmapSizeOptions.FromEmptyOptions());
                rImage.Freeze();
                r_TaskScreenshotTask.SetResult(rImage);

                NativeMethods.Gdi32.DeleteObject(rHBitmap);
            }

            BrowserService.Instance.Communicator.Write(CommunicatorMessages.FinishScreenshotTransmission);
        }
    }
}
