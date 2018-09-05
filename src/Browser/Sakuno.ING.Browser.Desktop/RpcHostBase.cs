using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace Sakuno.ING.Browser.Desktop
{
    public abstract class RpcHostBase : BrowserHost
    {
        public override void GoBack() => pipeStream.WriteByte((byte)RpcAction.GoBack);
        public override void GoForward() => pipeStream.WriteByte((byte)RpcAction.GoForward);
        public override void Navigate(string address)
        {
            var sendBuffer = new byte[address.Length * 2 + 4];
            sendBuffer[0] = (byte)RpcAction.Navigate;
            Buffer.BlockCopy(address.ToCharArray(), 0, sendBuffer, 4, address.Length * 2);
            pipeStream.Write(sendBuffer, 0, sendBuffer.Length);
        }
        public override void Refresh() => pipeStream.WriteByte((byte)RpcAction.Refresh);

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
