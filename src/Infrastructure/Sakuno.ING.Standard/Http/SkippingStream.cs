using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sakuno.ING.Http
{
    public sealed class SkippingStream : Stream
    {
        private readonly Stream original;
        private readonly int skipLength;

        public SkippingStream(Stream original, int skipLength)
        {
            this.original = original;
            this.skipLength = skipLength;
            original.Seek(skipLength, SeekOrigin.Begin);
        }

        public override bool CanRead => original.CanRead;
        public override bool CanSeek => original.CanSeek;
        public override bool CanTimeout => original.CanTimeout;
        public override bool CanWrite => false;
        public override long Length => original.Length - skipLength;

        public override long Position
        {
            get => original.Position - skipLength;
            set => original.Position = value + skipLength;
        }

        public override int ReadTimeout { get => original.ReadTimeout; set => original.ReadTimeout = value; }
        public override int WriteTimeout { get => original.WriteTimeout; set => original.WriteTimeout = value; }

        public override void Close() => original.Close();
        public override void Flush() => original.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => original.FlushAsync(cancellationToken);
        public override int Read(byte[] buffer, int offset, int count) => original.Read(buffer, offset, count);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => original.ReadAsync(buffer, offset, count, cancellationToken);
        public override int ReadByte() => original.ReadByte();
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin) offset += skipLength;
            return original.Seek(offset, origin) - skipLength;
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        protected override void Dispose(bool disposing) => original.Dispose();
    }
}
