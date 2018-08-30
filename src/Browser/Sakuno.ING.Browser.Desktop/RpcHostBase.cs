using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace Sakuno.ING.Browser.Desktop
{
    public abstract class RpcHostBase : BindableObject, IBrowserHost
    {
        public abstract UIElement BrowserElement { get; }

        private string _address;
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private bool _canGoBack;
        public bool CanGoBack
        {
            get => _canGoBack;
            set => Set(ref _canGoBack, value);
        }

        private bool _canGoForward;
        public bool CanGoForward
        {
            get => _canGoForward;
            set => Set(ref _canGoForward, value);
        }

        private bool _canRefresh;
        public bool CanRefresh
        {
            get => _canRefresh;
            set => Set(ref _canRefresh, value);
        }

        public void GoBack() => pipeStream.WriteByte((byte)RpcAction.GoBack);
        public void GoForward() => pipeStream.WriteByte((byte)RpcAction.GoForward);
        public void Navigate(string address)
        {
            var sendBuffer = new byte[address.Length*2 + 4];
            sendBuffer[0] = (byte)RpcAction.Navigate;
            Buffer.BlockCopy(address.ToCharArray(), 0, sendBuffer, 4, address.Length * 2);
            pipeStream.Write(sendBuffer, 0, sendBuffer.Length);
        }
        public void Refresh() => pipeStream.WriteByte((byte)RpcAction.Refresh);

        #region IDisposable
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                pipeStream.Dispose();
                remoteProcess.Kill();
                disposedValue = true;
            }
        }

        ~RpcHostBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        protected readonly NamedPipeServerStream pipeStream;
        protected readonly Process remoteProcess;
        protected RpcHostBase()
        {
            string pipeName = Path.GetRandomFileName();
            pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
            remoteProcess = CreateRemoteProcess(pipeName);
            PipeReceivingLoop();
        }

        protected abstract Process CreateRemoteProcess(string pipeName);

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
    }
}
