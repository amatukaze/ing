using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.SystemInterop;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class ScreenshotService
    {
        public static ScreenshotService Instance { get; } = new ScreenshotService();

        ListDictionary<long, TaskCompletionSource<BitmapSource>> r_TaskScreenshotTasks = new ListDictionary<long, TaskCompletionSource<BitmapSource>>();

        ScreenshotService()
        {
            var rMessages = BrowserService.Instance.Communicator.GetMessageObservable();
            rMessages.Subscribe(CommunicatorMessages.ScreenshotFail, ScreenshotFailed);
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
            var rTimestamp = DateTimeOffset.Now.Ticks;
            var rTaskScreenshotTask = new TaskCompletionSource<BitmapSource>();
            r_TaskScreenshotTasks.Add(rTimestamp, rTaskScreenshotTask);
            BrowserService.Instance.Communicator.Write(CommunicatorMessages.TakeScreenshot + ":" + rTimestamp.ToString());

            var rImage = await rTaskScreenshotTask.Task;
            if (rImage == null)
                throw new InvalidOperationException("No image data.");

            if (rpProcessAction != null)
                rImage = rpProcessAction(rImage);

            rTaskScreenshotTask = null;

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
                OutputException(e);
            }
        }
        public async void TakePartialScreenshotAndOutput(Int32Rect rpRect, bool rpOutputToClipboard)
        {
            try
            {
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
                OutputException(e);
            }
        }

        void ScreenshotFailed(string rpParameter)
        {
            var rParameters = rpParameter.Split(';');
            var rTimestamp = long.Parse(rParameters[0]);
            var rMessage = rParameters[1];

            r_TaskScreenshotTasks[rTimestamp].TrySetResult(null);
            r_TaskScreenshotTasks.Remove(rTimestamp);

            StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Log_Screenshot_Failed, rMessage);
        }

        public void OutputToClipboard(BitmapSource rpImage)
        {
            Clipboard.SetImage(rpImage);

            StatusBarService.Instance.Message = StringResources.Instance.Main.Log_Screenshot_Succeeded_Clipboard;
        }
        public void OutputAsFile(BitmapSource rpImage)
        {
            var rPreference = Preference.Current.Browser.Screenshot;

            string rExtension;
            BitmapEncoder rEncoder;
            switch (rPreference.ImageFormat.Value)
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

                default: throw new InvalidEnumArgumentException(nameof(rPreference.ImageFormat), (int)rPreference.ImageFormat.Value, typeof(ScreenshotImageFormat));
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

            StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Log_Screenshot_Succeeded_File, Path.GetFileName(rPath));
        }

        void GetScreenshot(string rpMapName, int rpWidth, int rpHeight, int rpBitCount)
        {
            var rTimestamp = long.Parse(rpMapName.Substring(rpMapName.LastIndexOf('/') + 1));
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
                r_TaskScreenshotTasks[rTimestamp].SetResult(rImage);

                NativeMethods.Gdi32.DeleteObject(rHBitmap);
            }

            BrowserService.Instance.Communicator.Write(CommunicatorMessages.FinishScreenshotTransmission);
        }

        static void OutputException(Exception rpException)
        {
            StatusBarService.Instance.Message = string.Format(StringResources.Instance.Main.Log_Screenshot_Failed, rpException.Message);

            try
            {
                using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                {
                    rStreamWriter.WriteLine("Screenshot error");
                    rStreamWriter.WriteLine();
                    rStreamWriter.WriteLine(rpException.ToString());
                }
            }
            catch { }
        }
    }
}
