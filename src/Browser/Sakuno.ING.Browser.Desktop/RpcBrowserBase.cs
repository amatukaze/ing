using System;
using System.IO.Pipes;

namespace Sakuno.ING.Browser.Desktop
{
    public abstract class RpcBrowserBase
    {
        protected readonly NamedPipeClientStream pipeStream;
        protected RpcBrowserBase(string pipeName)
        {
            pipeStream = new NamedPipeClientStream(",", pipeName, PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
            PipeReceivingLoop();
        }

        private async void PipeReceivingLoop()
        {
            await pipeStream.ConnectAsync();
            var buffer = new byte[2048];
            while (pipeStream.IsConnected)
            {
                int count = await pipeStream.ReadAsync(buffer, 0, buffer.Length);
                OnMessage(buffer, count);
            }
        }

        private void OnMessage(byte[] buffer, int count)
        {
            switch ((RpcAction)buffer[0])
            {
                case RpcAction.GoBack:
                    GoBack();
                    break;
                case RpcAction.GoForward:
                    GoForward();
                    break;
                case RpcAction.Refresh:
                    Refresh();
                    break;
                case RpcAction.Navigate:
                    var addr = new char[count / 2 - 2];
                    Buffer.BlockCopy(buffer, 4, addr, 0, count - 4);
                    Navigate(new string(addr));
                    break;
            }
        }

        protected bool CanGoBack { get; set; }
        protected bool CanGoForward { get; set; }
        protected bool CanRefresh { get; set; }
        protected string Address { get; set; }
        private readonly byte[] sendBuffer = new byte[2048];
        protected void SendState()
        {
            static byte EncodeBool(bool value) => value ? (byte)1 : (byte)0;

            sendBuffer[0] = EncodeBool(CanGoBack);
            sendBuffer[1] = EncodeBool(CanGoForward);
            sendBuffer[2] = EncodeBool(CanRefresh);
            string addr = Address;
            Buffer.BlockCopy(addr.ToCharArray(), 0, sendBuffer, 2, addr.Length * 2);
            pipeStream.Write(sendBuffer, 0, addr.Length * 2 + 2);
        }

        protected abstract void GoBack();
        protected abstract void GoForward();
        protected abstract void Refresh();
        protected abstract void Navigate(string address);
    }
}
