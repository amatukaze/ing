using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class ScreenshotService
    {
        public static ScreenshotService Instance { get; } = new ScreenshotService();

        public BitmapSource TakeScreenshot(Func<BitmapSource, BitmapSource> rpProcessAction = null)
        {
            var rBrowserWindow = ServiceManager.GetService<IBrowserService>().Handle;

            NativeStructs.RECT rRect;
            NativeMethods.User32.GetWindowRect(rBrowserWindow, out rRect);

            var rScreen = NativeMethods.User32.GetDC(rBrowserWindow);
            var rDC = NativeMethods.Gdi32.CreateCompatibleDC(rScreen);
            var rBitmap = NativeMethods.Gdi32.CreateCompatibleBitmap(rScreen, rRect.Width, rRect.Height);
            var rOldObject = NativeMethods.Gdi32.SelectObject(rDC, rBitmap);

            NativeMethods.Gdi32.BitBlt(rDC, 0, 0, rRect.Width, rRect.Height, rScreen, 0, 0, NativeConstants.RasterOperation.SRCCOPY);

            var rImage = Imaging.CreateBitmapSourceFromHBitmap(rBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            NativeMethods.Gdi32.SelectObject(rDC, rOldObject);
            NativeMethods.Gdi32.DeleteObject(rBitmap);
            NativeMethods.Gdi32.DeleteDC(rDC);
            NativeMethods.User32.ReleaseDC(IntPtr.Zero, rScreen);

            if (rpProcessAction != null)
                rImage = rpProcessAction(rImage);

            return rImage;
        }
        public BitmapSource TakePartialScreenshot(Int32Rect rpRect)
        {
            return TakeScreenshot(r =>
            {
                var rBrowserWindowHandle = ServiceManager.GetService<IBrowserService>().Handle;

                NativeStructs.RECT rBrowserWindowRect;
                NativeMethods.User32.GetWindowRect(rBrowserWindowHandle, out rBrowserWindowRect);

                var rHorizontalRatio = rBrowserWindowRect.Width / GameConstants.GameWidth;
                var rVerticalRatio = rBrowserWindowRect.Height / GameConstants.GameHeight;
                rpRect.X = (int)(rpRect.X * rHorizontalRatio);
                rpRect.Y = (int)(rpRect.Y * rVerticalRatio);
                rpRect.Width = (int)(rpRect.Width * rHorizontalRatio);
                rpRect.Height = (int)(rpRect.Height * rVerticalRatio);

                var rResult = new CroppedBitmap(r, rpRect);
                rResult.Freeze();

                return rResult;
            });
        }
        public void TakeScreenshotAndOutput(Func<BitmapSource, BitmapSource> rpProcessAction = null, bool rpOutputToClipboard = true)
        {
            try
            {
                var rImage = TakeScreenshot(rpProcessAction);
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
        public void TakePartialScreenshotAndOutput(Int32Rect rpRect, bool rpOutputToClipboard)
        {
            try
            {
                var rImage = TakePartialScreenshot(rpRect);
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
