using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Browser;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserWrapper
    {
        static DirectoryInfo r_BrowsersDirectory;

        string _layoutEngine;

        ContentControl r_Container;

        NamedPipeClientStream _namedPipeClient;

        StreamWriter _writer;
        BufferBlock<string> _messageQueue = new BufferBlock<string>();

        SortedList<string, object> _messageHandlers = new SortedList<string, object>(StringComparer.OrdinalIgnoreCase);

        HwndSource r_HwndSource;

        IBrowserProvider r_BrowserProvider;
        IBrowser r_Browser;

        double r_Zoom;

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
                {
                    try
                    {
                        rPath = Path.Combine(r_BrowsersDirectory.FullName, rPath);
                        if (File.Exists(rPath))
                            return Assembly.LoadFile(rPath);
                        else
                        {
                            var rSearchedFiles = r_BrowsersDirectory.GetFiles(Path.GetFileName(rPath), SearchOption.AllDirectories);
                            if (rSearchedFiles.Length > 0)
                                return Assembly.LoadFile(rSearchedFiles[0].FullName);
                            else
                                throw new FileNotFoundException($"Dependency not found: {rPath}");
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        throw new FileNotFoundException(rPath);
                    }
                }

                return null;
            };
        }
        public BrowserWrapper(string layoutEngine, int hostProcessId)
        {
            r_Container = new ContentControl();
            r_Container.PreviewKeyDown += (_, e) =>
            {
                if (e.Key == Key.System)
                    e.Handled = true;
            };

            _layoutEngine = layoutEngine;

            InitializeNamedPipe(hostProcessId);
        }

        void InitializeNamedPipe(int hostProcessId)
        {
            _namedPipeClient = new NamedPipeClientStream(".", $"Sakuno/HeavenlyWind({hostProcessId})", PipeDirection.InOut, PipeOptions.Asynchronous);

            RegisterAsyncMessageHandler(CommunicatorMessages.SetPort, parameter =>
            {
                try
                {
                    LoadBrowser(_layoutEngine);

                    r_BrowserProvider.SetPort(int.Parse(parameter));

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

                return SendMessage(CommunicatorMessages.Attach + ":" + r_HwndSource.Handle.ToInt32());
            });

            RegisterMessageHandler(CommunicatorMessages.ClearCache, _ => r_BrowserProvider?.ClearCache(false));
            RegisterMessageHandler(CommunicatorMessages.ClearCacheAndCookie, _ => r_BrowserProvider?.ClearCache(true));

            RegisterMessageHandler(CommunicatorMessages.GoBack, _ => r_Browser?.GoBack());
            RegisterMessageHandler(CommunicatorMessages.GoForward, _ => r_Browser?.GoForward());
            RegisterMessageHandler(CommunicatorMessages.Navigate, rpUrl => r_Browser?.Navigate(rpUrl));
            RegisterMessageHandler(CommunicatorMessages.Refresh, _ => r_Browser?.Refresh());

            RegisterAsyncMessageHandler(CommunicatorMessages.SetZoom, r =>
            {
                r_Zoom = double.Parse(r);
                r_Browser?.SetZoom(r_Zoom);

                return SendMessage(CommunicatorMessages.InvalidateArrange);
            });

            RegisterAsyncMessageHandler(CommunicatorMessages.ResizeBrowserToFitGame, delegate
            {
                r_Container.Width = GameConstants.GameWidth * r_Zoom / DpiUtil.ScaleX / DpiUtil.ScaleX;
                r_Container.Height = GameConstants.GameHeight * r_Zoom / DpiUtil.ScaleY / DpiUtil.ScaleY;

                return SendMessage(CommunicatorMessages.InvalidateArrange);
            });
        }

        public void RegisterMessageHandler(string command, Action<string> handler) => _messageHandlers.Add(command, handler);
        public void RegisterAsyncMessageHandler(string command, Func<string, Task> handler) => _messageHandlers.Add(command, handler);

        public Task SendMessage(string message) => _messageQueue.SendAsync(message);
        async void ConsumeMessageQueue()
        {
            while (true)
            {
                var message = await _messageQueue.ReceiveAsync();

                await _writer.WriteLineAsync(message);
                await _writer.FlushAsync();
            }
        }

        public void Connect()
        {
            _namedPipeClient.Connect();

            _writer = new StreamWriter(_namedPipeClient);

            ConsumeMessageQueue();
            MessageReceiverCore();
        }
        async void MessageReceiverCore()
        {
            await SendMessage(CommunicatorMessages.Ready);

            var reader = new StreamReader(_namedPipeClient);

            try
            {
                while (true)
                {
                    var message = await reader.ReadLineAsync();
                    if (message == null)
                        return;

                    var command = message;
                    var parameter = string.Empty;

                    var position = message.IndexOf(':');
                    if (position != -1)
                    {
                        command = message.Remove(position);
                        parameter = message.Substring(position + 1);
                    }

                    if (_messageHandlers.TryGetValue(command, out var value))
                        switch (value)
                        {
                            case Action<string> syncHandler:
                                syncHandler(parameter);
                                break;

                            case Func<string, Task> asyncHandler:
                                await asyncHandler(parameter);
                                break;
                        }
                }
            }
            catch (IOException e)
            {
                var rDialog = new TaskDialog()
                {
                    Caption = UnhandledExceptionDialogStringResources.ProductName,
                    Instruction = UnhandledExceptionDialogStringResources.Instruction,
                    Icon = TaskDialogIcon.Error,
                    Content = UnhandledExceptionDialogStringResources.Content,

                    Detail = e.ToString(),
                    ShowDetailAtTheBottom = true,

                    ShowAtTheCenterOfOwner = true,
                };

                rDialog.ShowAndDispose();

                Process.GetCurrentProcess().Kill();
            }
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

            r_Browser.LoadCompleted += async (rpCanGoBack, rpCanGoForward, rpUrl) =>
            {
                await SendMessage(CommunicatorMessages.LoadCompleted + $":{rpCanGoBack};{rpCanGoForward};{rpUrl}");

                if (rpUrl == GameConstants.GamePageUrl || rpUrl.Contains(".swf"))
                    await SendMessage(CommunicatorMessages.LoadGamePageCompleted);
            };
        }

        void LoadBrowser(string rpLayoutEngine)
        {
            if (!r_BrowsersDirectory.Exists)
                throw new DirectoryNotFoundException(r_BrowsersDirectory.FullName);

            foreach (var rFile in r_BrowsersDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                FileSystem.Unblock(rFile.FullName);

            var rInfos = r_BrowsersDirectory.EnumerateFiles("*.json").Select(r =>
            {
                using (var rReader = new JsonTextReader(File.OpenText(r.FullName)))
                    return JObject.Load(rReader).ToObject<LayoutEngineInfo>();
            }).ToArray();
            var rInfo = rInfos.SingleOrDefault(r => r.Name == rpLayoutEngine);

            if (rInfo == null)
                throw new Exception($"Assigned layout engine not found.{Environment.NewLine}Current layout engine: {rpLayoutEngine}{Environment.NewLine}Installed layout engine(s):{Environment.NewLine}{rInfos.Select(r => r.Name).Join(Environment.NewLine)}");

            if (rInfo.Dependencies != null)
                r_LayoutEngineDependencies = rInfo.Dependencies.ToHybridDictionary(r => r.AssemblyName, r => r.Path);

            var rAssembly = Assembly.LoadFile(Path.Combine(r_BrowsersDirectory.FullName, rInfo.EntryFile));
            var rType = rAssembly.GetTypes().Where(r => r.GetInterface(typeof(IBrowserProvider).FullName) != null).FirstOrDefault();

            if (rType == null)
                throw new Exception($"IBrowserProvider not found in {rAssembly.FullName}.");

            r_BrowserProvider = (IBrowserProvider)rAssembly.CreateInstance(rType.FullName);
        }
    }
}
