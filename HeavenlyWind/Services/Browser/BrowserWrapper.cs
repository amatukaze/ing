using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Browser;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserWrapper
    {
        static DirectoryInfo r_BrowsersDirectory;

        ContentControl r_Container;

        MemoryMappedFileCommunicator r_Communicator;
        IConnectableObservable<KeyValuePair<string, string>> r_Messages;

        HwndSource r_HwndSource;

        IBrowserProvider r_BrowserProvider;
        IBrowser r_Browser;

        double r_Zoom;

        MemoryMappedFile r_ScreenshotMMF;

        static HybridDictionary<string, string> r_LayoutEngineDependencies;

        static BrowserWrapper()
        {
            r_BrowsersDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "Browsers"));

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var rName = e.Name;
                var rPosition = rName.IndexOf(',');
                if (rPosition != -1)
                    rName = rName.Remove(rPosition);

                string rPath;
                if (r_LayoutEngineDependencies != null && r_LayoutEngineDependencies.TryGetValue(rName, out rPath))
                    return Assembly.LoadFile(Path.Combine(r_BrowsersDirectory.FullName, rPath));

                return null;
            };
        }
        public BrowserWrapper(string rpLayoutEngine, int rpHostProcessID)
        {
            r_Container = new ContentControl();

            InitializeCommunicator(rpHostProcessID);

            r_Messages.Subscribe(CommunicatorMessages.SetPort, r =>
            {
                try
                {
                    LoadBrowser(rpLayoutEngine);

                    r_BrowserProvider.SetPort(int.Parse(r));

                    InitializeBrowserControl();
                    r_Container.Content = r_Browser;
                }
                catch (ReflectionTypeLoadException e)
                {
                    r_Container.Content = e.LoaderExceptions[0].ToString();
                }
                catch (Exception e)
                {
                    r_Container.Content = e.ToString();
                }

                InitializeHwndSource();
                r_Communicator.Write(CommunicatorMessages.Attach + ":" + r_HwndSource.Handle.ToInt32());

            });

            r_Communicator.Write(CommunicatorMessages.Ready);
        }

        void InitializeCommunicator(int rpHostProcessID)
        {
            r_Communicator = new MemoryMappedFileCommunicator($"Sakuno/HeavenlyWind({rpHostProcessID})", 4096);
            r_Communicator.ReadPosition = 0;
            r_Communicator.WritePosition = 2048;

            r_Messages = r_Communicator.GetMessageObservable().ObserveOnDispatcher().Publish();
            r_Messages.Connect();

            r_Communicator.StartReader();

            r_Messages.Subscribe(CommunicatorMessages.ClearCache, _ => r_BrowserProvider?.ClearCache(false));
            r_Messages.Subscribe(CommunicatorMessages.ClearCacheAndCookie, _ => r_BrowserProvider?.ClearCache(true));

            r_Messages.Subscribe(CommunicatorMessages.GoBack, _ => r_Browser?.GoBack());
            r_Messages.Subscribe(CommunicatorMessages.GoForward, _ => r_Browser?.GoForward());
            r_Messages.Subscribe(CommunicatorMessages.Navigate, rpUrl => r_Browser?.Navigate(rpUrl));
            r_Messages.Subscribe(CommunicatorMessages.Refresh, _ => r_Browser?.Refresh());

            r_Messages.Subscribe(CommunicatorMessages.SetZoom, r =>
            {
                r_Zoom = double.Parse(r);
                r_Browser?.SetZoom(r_Zoom);
                r_Communicator.Write(CommunicatorMessages.InvalidateArrange);
            });

            r_Messages.Subscribe(CommunicatorMessages.ResizeBrowserToFitGame, _ =>
            {
                r_Container.Width = GameConstants.GameWidth * r_Zoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                r_Container.Height = GameConstants.GameHeight * r_Zoom / DpiUtil.ScaleY / DpiUtil.ScaleY;
                r_Communicator.Write(CommunicatorMessages.InvalidateArrange);
            });

            InitializeScreenshotMessagesSubscription();

        }

        void InitializeHwndSource()
        {
            var rParameters = new HwndSourceParameters("HeavenlyWind Browser Window") { WindowStyle = 0 };
            r_HwndSource = new HwndSource(rParameters);
            r_HwndSource.CompositionTarget.BackgroundColor = Colors.White;

            r_HwndSource.AddHook(WndProc);

            NativeMethods.User32.SetWindowLongPtr(r_HwndSource.Handle, NativeConstants.GetWindowLong.GWL_STYLE, (IntPtr)(NativeEnums.WindowStyle.WS_CHILD | NativeEnums.WindowStyle.WS_CLIPCHILDREN));
            NativeMethods.User32.SetWindowPos(r_HwndSource.Handle, IntPtr.Zero, 0, 0, 0, 0, NativeEnums.SetWindowPosition.SWP_FRAMECHANGED | NativeEnums.SetWindowPosition.SWP_NOSIZEORMOVE | NativeEnums.SetWindowPosition.SWP_NOZORDER);

            r_HwndSource.RootVisual = r_Container;
        }
        IntPtr WndProc(IntPtr rpHandle, int rpMessage, IntPtr rpWParam, IntPtr rpLParam, ref bool rrpHandled)
        {
            var rMessage = (NativeConstants.WindowMessage)rpMessage;
            if (rMessage == CommunicatorMessages.ResizeBrowserWindow)
            {
                r_Container.Width = rpWParam.ToInt32();
                r_Container.Height = rpLParam.ToInt32();

                NativeMethods.User32.SetWindowPos(r_HwndSource.Handle, IntPtr.Zero, 0, 0, rpWParam.ToInt32(), rpLParam.ToInt32(), NativeEnums.SetWindowPosition.SWP_NOMOVE | NativeEnums.SetWindowPosition.SWP_NOZORDER);

                rrpHandled = true;
            }

            return IntPtr.Zero;
        }

        void InitializeBrowserControl()
        {
            r_Browser = r_BrowserProvider.CreateBrowserInstance();

            r_Browser.LoadCompleted += (rpCanGoBack, rpCanGoForward, rpUrl) =>
            {
                r_Communicator.Write(CommunicatorMessages.LoadCompleted + $":{rpCanGoBack};{rpCanGoForward};{rpUrl}");
                if (rpUrl == GameConstants.GamePageUrl || rpUrl.Contains(".swf"))
                    r_Communicator.Write(CommunicatorMessages.LoadGamePageCompleted);
            };
        }

        void LoadBrowser(string rpLayoutEngine)
        {
            if (!r_BrowsersDirectory.Exists)
                throw new Exception();

            foreach (var rFile in r_BrowsersDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                FileSystem.Unblock(rFile.FullName);

            var rInfo = r_BrowsersDirectory.EnumerateFiles("*.json").Select(r =>
            {
                using (var rReader = new JsonTextReader(File.OpenText(r.FullName)))
                    return JObject.Load(rReader).ToObject<LayoutEngineInfo>();
            }).SingleOrDefault(r => r.Name == rpLayoutEngine);

            if (rInfo == null)
                throw new Exception();

            if (rInfo.Dependencies != null)
                r_LayoutEngineDependencies = rInfo.Dependencies.ToHybridDictionary(r => r.AssemblyName, r => r.Path);

            var rAssembly = Assembly.LoadFile(Path.Combine(r_BrowsersDirectory.FullName, rInfo.EntryFile));
            var rType = rAssembly.GetTypes().Where(r => r.GetInterface(typeof(IBrowserProvider).FullName) != null).FirstOrDefault();

            if (rType == null)
                throw new Exception();

            r_BrowserProvider = (IBrowserProvider)rAssembly.CreateInstance(rType.FullName);
        }

        void InitializeScreenshotMessagesSubscription()
        {
            r_Messages.Where(r => r.Key == CommunicatorMessages.TakeScreenshot).Subscribe(delegate
            {
                try
                {
                    var rScreenshotData = r_Browser.TakeScreenshot();
                    if (rScreenshotData == null || rScreenshotData.BitmapData == null)
                    {
                        r_Communicator.Write(CommunicatorMessages.ScreenshotFail);
                        r_ScreenshotMMF = null;
                    }

                    const string MapName = "HeavenlyWind/ScreenshotTransmission";
                    var rMemoryMappedFile = MemoryMappedFile.CreateNew(MapName, rScreenshotData.BitmapData.Length, MemoryMappedFileAccess.ReadWrite);
                    using (var rStream = rMemoryMappedFile.CreateViewStream())
                        rStream.Write(rScreenshotData.BitmapData, 0, rScreenshotData.BitmapData.Length);

                    r_Communicator.Write(CommunicatorMessages.StartScreenshotTransmission + $":{MapName};{rScreenshotData.Width};{rScreenshotData.Height};{rScreenshotData.BitCount}");

                    r_ScreenshotMMF = rMemoryMappedFile;
                }
                catch (Exception e)
                {
                    r_Communicator.Write(CommunicatorMessages.ScreenshotFail + ":" + e.Message);
                    r_ScreenshotMMF = null;
                }
            });
            r_Messages.Where(r => r.Key == CommunicatorMessages.FinishScreenshotTransmission).Subscribe(delegate
            {
                if (r_ScreenshotMMF != null)
                {
                    r_ScreenshotMMF.Dispose();
                    r_ScreenshotMMF = null;
                }
            });
        }

    }
}
