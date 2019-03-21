using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Windows;
using System.Windows.Interop;

namespace Sakuno.ING.Browser.Desktop
{
    public abstract class RpcHostBase : HwndHost, IBrowser
    {
        public void GoBack() => pipeStream.WriteByte((byte)RpcAction.GoBack);
        public void GoForward() => pipeStream.WriteByte((byte)RpcAction.GoForward);
        public void Navigate(string address)
        {
            var sendBuffer = new byte[address.Length * 2 + 4];
            sendBuffer[0] = (byte)RpcAction.Navigate;
            Buffer.BlockCopy(address.ToCharArray(), 0, sendBuffer, 4, address.Length * 2);
            pipeStream.Write(sendBuffer, 0, sendBuffer.Length);
        }
        public void Refresh() => pipeStream.WriteByte((byte)RpcAction.Refresh);

        protected override void Dispose(bool disposing)
        {
            try
            {
                pipeStream.Dispose();
                remoteProcess.Kill();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected readonly NamedPipeServerStream pipeStream;
        protected readonly Process remoteProcess;
        protected RpcHostBase(int port)
        {
            string pipeName = Path.GetRandomFileName();
            pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
            remoteProcess = CreateRemoteProcess(pipeName, port);
            remoteProcess.Exited += (_, __) => BrowserExited?.Invoke();
            PipeReceivingLoop();
        }

        protected abstract Process CreateRemoteProcess(string pipeName, int port);

        private async void PipeReceivingLoop()
        {
            await pipeStream.WaitForConnectionAsync();
            var buffer = new byte[2048];
            while (pipeStream.IsConnected)
            {
                int count = await pipeStream.ReadAsync(buffer, 0, buffer.Length);
                OnMessage(buffer, count);
            }

            Address = null;
            CanGoBack = false;
            CanGoForward = false;
            CanRefresh = false;
        }

        private void OnMessage(byte[] buffer, int count)
        {
            CanGoBack = buffer[0] != 0;
            CanGoForward = buffer[1] != 0;
            CanRefresh = buffer[2] != 0;
            var addr = new char[count / 2 - 2];
            Buffer.BlockCopy(buffer, 4, addr, 0, count - 4);
            Address = new string(addr);
        }

        public void ScaleTo(double scale) => throw new NotImplementedException();

        public static readonly DependencyProperty AddressProperty
            = DependencyProperty.Register(nameof(Address), typeof(string), typeof(RpcHostBase), new PropertyMetadata(string.Empty));
        public string Address
        {
            get => (string)GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        public static readonly DependencyProperty CanGoBackProperty
            = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(RpcHostBase), new PropertyMetadata(false));
        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty);
            set => SetValue(CanGoBackProperty, value);
        }

        public static readonly DependencyProperty CanGoForwardProperty
            = DependencyProperty.Register(nameof(CanGoForward), typeof(bool), typeof(RpcHostBase), new PropertyMetadata(false));
        public bool CanGoForward
        {
            get => (bool)GetValue(CanGoForwardProperty);
            set => SetValue(CanGoForwardProperty, value);
        }

        public static readonly DependencyProperty CanRefreshProperty
            = DependencyProperty.Register(nameof(CanRefresh), typeof(bool), typeof(RpcHostBase), new PropertyMetadata(false));

        public bool CanRefresh
        {
            get => (bool)GetValue(CanRefreshProperty);
            set => SetValue(CanRefreshProperty, value);
        }

        public event Action BrowserExited;
    }
}
