using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class BrowserService : ModelBase, IBrowserService
    {
        public static BrowserService Instance { get; } = new BrowserService();

        NamedPipeServerStream _namedPipeServer;
        StreamWriter _writer;
        SortedList<string, List<object>> _messageHandlers = new SortedList<string, List<object>>(StringComparer.OrdinalIgnoreCase);

        BufferBlock<string> _messageQueue = new BufferBlock<string>();

        public IList<LayoutEngineInfo> InstalledLayoutEngines { get; private set; }

        public int HostProcessID => Process.GetCurrentProcess().Id;

        bool r_Initialized;

        bool r_NoInstalledLayoutEngines;
        public bool NoInstalledLayoutEngines
        {
            get { return r_NoInstalledLayoutEngines; }
            private set
            {
                if (r_NoInstalledLayoutEngines != value)
                {
                    r_NoInstalledLayoutEngines = value;
                    OnPropertyChanged(nameof(NoInstalledLayoutEngines));
                }
            }
        }

        Process r_BrowserProcess;
        public int? BrowserProcessID => r_BrowserProcess?.Id;

        public BrowserHost BrowserControl { get; private set; }
        public IntPtr Handle => BrowserControl != null ? BrowserControl.BrowserHandle : IntPtr.Zero;

        public BrowserNavigator Navigator { get; private set; }

        bool r_IsNavigatorVisible;
        public bool IsNavigatorVisible
        {
            get { return r_IsNavigatorVisible; }
            private set
            {
                if (r_IsNavigatorVisible != value)
                {
                    r_IsNavigatorVisible = value;
                    OnPropertyChanged(nameof(IsNavigatorVisible));
                }
            }
        }

        public bool IsResizedToFitGame { get; private set; }

        public GameController GameController { get; private set; }

        public ICommand ClearCacheCommand { get; }
        public ICommand ClearCookieCommand { get; }

        public event Action Attached;
        public event EventHandler<Size> Resized;
        public event Action ResizedToFitGame;

        TaskCompletionSource<string> _screenshotTcs;

        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, NativeEnums.SFGAO sfgaoIn, out NativeEnums.SFGAO psfgaoOut);
        [DllImport("shell32.dll")]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, int dwFlags);

        BrowserService()
        {
            r_IsNavigatorVisible = true;

            ClearCacheCommand = new DelegatedCommand(() => SendMessage(CommunicatorMessages.ClearCache).Forget());
            ClearCookieCommand = new DelegatedCommand(() => SendMessage(CommunicatorMessages.ClearCookie).Forget());
        }

        public void Initialize()
        {
            if (!r_Initialized)
            {
                if (!LoadLayoutEngines())
                {
                    NoInstalledLayoutEngines = true;
                    r_Initialized = true;
                    return;
                }

                InitializeNamedPipe();

                var rStartInfo = new ProcessStartInfo()
                {
                    FileName = typeof(BrowserService).Assembly.Location,
                    Arguments = $"browser {Preference.Instance.Browser.CurrentLayoutEngine} {HostProcessID}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };
                r_BrowserProcess = Process.Start(rStartInfo);
                r_BrowserProcess.BeginOutputReadLine();
                r_BrowserProcess.OutputDataReceived += (s, e) => Trace.WriteLine(e.Data);

                RegisterAsyncMessageHandler(CommunicatorMessages.Ready, async _ =>
                {
                    await SendMessage(CommunicatorMessages.Initialize);

                    if (Preference.Instance.Browser.CurrentLayoutEngine == "blink")
                        await SendMessage(CommunicatorMessages.InitializeBlink + ":" + Preference.Instance.Browser.Blink.DisableHWA.Value.ToString());

                    await SendMessage(CommunicatorMessages.SetPort + ":" + Preference.Instance.Network.Port);
                });
                RegisterAsyncMessageHandler(CommunicatorMessages.Attach, parameter => Attach((IntPtr)int.Parse(parameter)));

                r_Initialized = true;

                RegisterAsyncMessageHandler(CommunicatorMessages.LoadCompleted, _ => SendMessage(CommunicatorMessages.SetZoom + ":1.0"));
                RegisterAsyncMessageHandler(CommunicatorMessages.LoadGamePageCompleted, _ => ResizeBrowserToFitGame());

                RegisterMessageHandler(CommunicatorMessages.ScreenshotData, data => Interlocked.Exchange(ref _screenshotTcs, null)?.SetResult(data));

                Navigator = new BrowserNavigator(this);
                GameController = new GameController(this);

                ConsumeMessageQueue();
                MessageReceiverCore();
            }
        }
        bool LoadLayoutEngines()
        {
            var rBrowsersDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "Browsers"));
            if (!rBrowsersDirectory.Exists)
                return false;

            try
            {
                var rInstalledLayoutEngines = rBrowsersDirectory.EnumerateFiles("*.json").Select(r =>
                {
                    using (var rReader = new JsonTextReader(File.OpenText(r.FullName)))
                        return JObject.Load(rReader).ToObject<LayoutEngineInfo>();
                });
                InstalledLayoutEngines = rInstalledLayoutEngines.ToList();

                if (InstalledLayoutEngines.Count == 0)
                    return false;

                var rSelectedLayoutEngine = Preference.Instance.Browser.CurrentLayoutEngine.Value;
                if (InstalledLayoutEngines.FirstOrDefault(r => r.Name == rSelectedLayoutEngine) == null)
                    Preference.Instance.Browser.CurrentLayoutEngine.Value = InstalledLayoutEngines[0].Name;
            }
            catch
            {
                return false;
            }

            return true;
        }

        void InitializeNamedPipe()
        {
            _namedPipeServer = new NamedPipeServerStream($"Sakuno/HeavenlyWind({HostProcessID})", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public void RegisterMessageHandler(string command, Action<string> handler) => RegisterMessageHandlerCore(command, handler);
        public void RegisterAsyncMessageHandler(string command, Func<string, Task> handler) => RegisterMessageHandlerCore(command, handler);
        void RegisterMessageHandlerCore(string command, object handler)
        {
            if (!_messageHandlers.TryGetValue(command, out var handlers))
                _messageHandlers.Add(command, handlers = new List<object>(1));

            handlers.Add(handler);
        }

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

        async void MessageReceiverCore()
        {
            await Task.Factory.FromAsync(_namedPipeServer.BeginWaitForConnection, _namedPipeServer.EndWaitForConnection, null);

            _writer = new StreamWriter(_namedPipeServer);

            var reader = new StreamReader(_namedPipeServer);

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

                if (_messageHandlers.TryGetValue(command, out var handlers))
                    foreach (var handler in handlers)
                        switch (handler)
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

        async Task Attach(IntPtr rpHandle)
        {
            BrowserControl = new BrowserHost(rpHandle);
            OnPropertyChanged(nameof(BrowserControl));

            Attached?.Invoke();

            BrowserControl.SizeChanged += (s, e) => Resized?.Invoke(this, e.NewSize);

            await Task.Delay(2000);

            Navigator.Navigate(Preference.Instance.Browser.Homepage);

            if (Preference.Instance.Browser.CurrentLayoutEngine == "blink")
                Preference.Instance.Browser.Blink.MaxFramerate.Subscribe(value =>
                    SendMessage(CommunicatorMessages.SetBlinkMaxFramerate + ":" + value.ToString()), true);
        }

        internal async Task ResizeBrowserToFitGame()
        {
            if (BrowserControl == null)
                return;

            await SendMessage(CommunicatorMessages.SetZoom + ":" + Preference.Instance.Browser.Zoom);

            if (Preference.Instance.Browser.CurrentLayoutEngine == "blink")
                await SendMessage(CommunicatorMessages.SetBlinkMaxFramerate + ":" + Preference.Instance.Browser.Blink.MaxFramerate.Value.ToString());

            await SendMessage(CommunicatorMessages.ResizeBrowserToFitGame);

            IsNavigatorVisible = false;

            BrowserControl.ResizeBrowserToFitGame();

            IsResizedToFitGame = true;
            OnPropertyChanged(nameof(IsResizedToFitGame));

            Resized?.Invoke(this, BrowserControl.DesiredSize);

            ResizedToFitGame?.Invoke();
        }

        public void SetDefaultHomepage(string rpUrl) => Preference.Instance.Browser.Homepage.Value = rpUrl;

        public async Task<string> TakeScreenshot()
        {
            var screenshotTcs = new TaskCompletionSource<string>();
            _screenshotTcs = screenshotTcs;

            await SendMessage(CommunicatorMessages.TakeScreenshot);

            return await screenshotTcs.Task;
        }

        public void Shutdown() => SendMessage(CommunicatorMessages.Shutdown);
    }
}
