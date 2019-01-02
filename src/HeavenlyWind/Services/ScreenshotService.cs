using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop;
using System;
using System.ComponentModel;
using System.IO;
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

        public async Task<BitmapSource> TakeScreenshot()
        {
            var data = await BrowserService.Instance.TakeScreenshot();

            var s = data.Substring(data.IndexOf(',') + 1);
            var bytes = Convert.FromBase64String(s);

            return BitmapFrame.Create(new MemoryStream(bytes));
        }
        public async Task<BitmapSource> TakePartialScreenshot(Int32Rect rpRect)
        {
            var original = await TakeScreenshot();

            var rBrowserWindowHandle = ServiceManager.GetService<IBrowserService>().Handle;

            NativeMethods.User32.GetWindowRect(rBrowserWindowHandle, out var rBrowserWindowRect);

            var rHorizontalRatio = rBrowserWindowRect.Width / GameConstants.GameWidth;
            var rVerticalRatio = rBrowserWindowRect.Height / GameConstants.GameHeight;
            rpRect.X = (int)(rpRect.X * rHorizontalRatio);
            rpRect.Y = (int)(rpRect.Y * rVerticalRatio);
            rpRect.Width = (int)(rpRect.Width * rHorizontalRatio);
            rpRect.Height = (int)(rpRect.Height * rVerticalRatio);

            var rResult = new CroppedBitmap(original, rpRect);
            rResult.Freeze();

            return rResult;
        }
        public async void TakeScreenshotAndOutput(bool rpOutputToClipboard = true)
        {
            try
            {
                var rImage = await TakeScreenshot();
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

        public void OutputToClipboard(BitmapSource rpImage)
        {
            Clipboard.SetImage(rpImage);

            StatusBarService.Instance.Message = StringResources.Instance.Main.Log_Screenshot_Succeeded_Clipboard;
        }
        public void OutputAsFile(BitmapSource rpImage)
        {
            var rPreference = Preference.Instance.Browser.Screenshot;

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

            var rPath = Path.Combine(rPreference.Path, string.Format(rPreference.FilenameFormat, DateTime.Now, rExtension));
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
