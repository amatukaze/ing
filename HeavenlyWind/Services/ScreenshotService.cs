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
            rMessages.Subscribe(CommunicatorMessages.ScreenshotFail, _ => ScreenshotFailed());
            rMessages.Subscribe(CommunicatorMessages.StartScreenshotTransmission, rpParameter =>
            {
                var rParameters = rpParameter.Split(';');
                if (rParameters.Length < 4)
                    return;

                var rMapName = rParameters[0];
                var rWidth = int.Parse(rParameters[1]);
                var rHeight = int.Parse(rParameters[2]);
                var rBitCount = int.Parse(rParameters[3]);
                GetScreenshot(rMapName, rWidth, rHeight, rBitCount);
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
            try
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
            catch (Exception e)
            {
                StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Screenshot_Failed, e.Message);
            }
        }
        public async void TakePartialScreenshotAndOutput(Int32Rect rpRect, bool rpOutputToClipboard)
        {
            try
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
            catch (Exception e)
            {
                StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Screenshot_Failed, e.Message);
            }
        }

        void ScreenshotFailed()
        {
            r_TaskScreenshotTask.SetResult(null);

            StatusBarService.Instance.Message = StringResources.Instance.Main.Screenshot_Failed;
        }

        public void OutputToClipboard(BitmapSource rpImage)
        {
            Clipboard.SetImage(rpImage);

            StatusBarService.Instance.Message = StringResources.Instance.Main.Screenshot_Succeeded_Clipboard;
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

            StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Screenshot_Succeeded_File, Path.GetFileName(rPath));
        }

        void GetScreenshot(string rpMapName, int rpWidth, int rpHeight, int rpBitCount)
        {
            var rHeight = Math.Abs(rpHeight);

            using (var rMap = MemoryMappedFile.CreateOrOpen(rpMapName, rpWidth * rHeight * 3, MemoryMappedFileAccess.ReadWrite))
            {
                var rInfo = new NativeStructs.BITMAPINFO();
                rInfo.bmiHeader.biSize = Marshal.SizeOf(typeof(NativeStructs.BITMAPINFOHEADER));
                rInfo.bmiHeader.biWidth = rpWidth;
                rInfo.bmiHeader.biHeight = rpHeight;
                rInfo.bmiHeader.biBitCount = (ushort)rpBitCount;
                rInfo.bmiHeader.biPlanes = 1;

                IntPtr rBits;
                var rHBitmap = NativeMethods.Gdi32.CreateDIBSection(IntPtr.Zero, ref rInfo, 0, out rBits, rMap.SafeMemoryMappedFileHandle.DangerousGetHandle(), 0);
                if (rHBitmap == IntPtr.Zero)
                    throw new InvalidOperationException();

                var rImage = Imaging.CreateBitmapSourceFromHBitmap(rHBitmap, IntPtr.Zero, new Int32Rect(0, 0, rpWidth, rHeight), BitmapSizeOptions.FromEmptyOptions());
                rImage.Freeze();
                r_TaskScreenshotTask.SetResult(rImage);

                NativeMethods.Gdi32.DeleteObject(rHBitmap);
            }

            BrowserService.Instance.Communicator.Write(CommunicatorMessages.FinishScreenshotTransmission);
        }
    }
}
