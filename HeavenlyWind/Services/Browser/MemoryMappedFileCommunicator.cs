using System;
using System.IO.MemoryMappedFiles;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class MemoryMappedFileCommunicator : IDisposable
    {
        const int DATA_AVAILABLE_OFFSET = 0;
        const int READ_CONFIRM_OFFSET = DATA_AVAILABLE_OFFSET + 1;
        const int DATA_LENGTH_OFFSET = READ_CONFIRM_OFFSET + 1;
        const int DATA_OFFSET = 16;

        public int ReadPosition { get; set; }

        int r_WritePosition;
        public int WritePosition
        {
            get { return r_WritePosition; }
            set
            {
                if (r_WritePosition != value)
                {
                    r_WritePosition = value;
                    r_ViewAccessor.Write(WritePosition + READ_CONFIRM_OFFSET, true);
                }
            }
        }

        MemoryMappedFile r_MemoryMappedFile;
        MemoryMappedViewAccessor r_ViewAccessor;
        bool r_IsStarted;
        bool r_IsDisposed;

        Task r_ReadTask;

        public Subject<byte[]> DataReceived { get; } = new Subject<byte[]>();

        public MemoryMappedFileCommunicator(string rpMapName, long rpCapacity)
            : this(MemoryMappedFile.CreateOrOpen(rpMapName, rpCapacity), 0, 0, MemoryMappedFileAccess.ReadWrite) { }
        public MemoryMappedFileCommunicator(string rpMapName, long rpCapacity, long rpOffset, long rpSize)
            : this(MemoryMappedFile.CreateOrOpen(rpMapName, rpCapacity), rpOffset, rpSize, MemoryMappedFileAccess.ReadWrite) { }
        public MemoryMappedFileCommunicator(string rpMapName, long rpCapacity, long rpOffset, long rpSize, MemoryMappedFileAccess rpAccess)
            : this(MemoryMappedFile.CreateOrOpen(rpMapName, rpCapacity), rpOffset, rpSize, rpAccess) { }
        public MemoryMappedFileCommunicator(MemoryMappedFile rpMemoryMappedFile)
            : this(rpMemoryMappedFile, 0, 0, MemoryMappedFileAccess.ReadWrite) { }
        public MemoryMappedFileCommunicator(MemoryMappedFile rpMemoryMappedFile, long rpOffset, long rpSize)
            : this(rpMemoryMappedFile, rpOffset, rpSize, MemoryMappedFileAccess.ReadWrite) { }
        public MemoryMappedFileCommunicator(MemoryMappedFile rpMemoryMappedFile, long rpOffset, long rpSize, MemoryMappedFileAccess rpAccess)
        {
            r_MemoryMappedFile = rpMemoryMappedFile;
            r_ViewAccessor = rpMemoryMappedFile.CreateViewAccessor(rpOffset, rpSize, rpAccess);

            ReadPosition = -1;
            r_WritePosition = -1;
        }

        public void StartReader()
        {
            if (r_IsDisposed || r_IsStarted)
                return;

            if (ReadPosition < 0 || r_WritePosition < 0)
                throw new ArgumentException();

            if (r_ReadTask == null)
            {
                r_IsStarted = true;
                r_ReadTask = Task.Factory.StartNew(ReadCore, TaskCreationOptions.LongRunning);
            }
        }
        public void StopReader() => r_IsStarted = false;
        async void ReadCore()
        {
            while (r_IsStarted)
            {
                var rAvailable = r_ViewAccessor.ReadBoolean(ReadPosition + DATA_AVAILABLE_OFFSET);
                if (rAvailable)
                {
                    var rLength = r_ViewAccessor.ReadInt32(ReadPosition + DATA_LENGTH_OFFSET);
                    var rData = new byte[rLength];
                    var rBytesRead = r_ViewAccessor.ReadArray<byte>(ReadPosition + DATA_OFFSET, rData, 0, rLength);

                    r_ViewAccessor.Write(ReadPosition + DATA_AVAILABLE_OFFSET, false);
                    r_ViewAccessor.Write(ReadPosition + READ_CONFIRM_OFFSET, true);

                    DataReceived.OnNext(rData);
                }

                await Task.Delay(500);
            }
        }

        public void Write(string rpMessage) => Write(Encoding.UTF8.GetBytes(rpMessage));
        public void Write(byte[] rpData)
        {
            if (ReadPosition < 0 || r_WritePosition < 0)
                throw new ArgumentException();

            WriteCore(rpData);
        }
        void WriteCore(byte[] rpData)
        {
            if (r_IsDisposed)
                return;

            while (!r_ViewAccessor.ReadBoolean(r_WritePosition + READ_CONFIRM_OFFSET))
                Thread.Sleep(500);

            r_ViewAccessor.Write(r_WritePosition + DATA_LENGTH_OFFSET, rpData.Length);
            r_ViewAccessor.WriteArray<byte>(r_WritePosition + DATA_OFFSET, rpData, 0, rpData.Length);

            r_ViewAccessor.Write(r_WritePosition + READ_CONFIRM_OFFSET, false);
            r_ViewAccessor.Write(r_WritePosition + DATA_AVAILABLE_OFFSET, true);
        }

        public void Dispose()
        {
            if (r_IsDisposed) return;

            r_IsStarted = false;

            if (r_ViewAccessor != null)
            {
                r_ViewAccessor.Dispose();
                r_ViewAccessor = null;
            }
            if (r_MemoryMappedFile != null)
            {
                r_MemoryMappedFile.Dispose();
                r_MemoryMappedFile = null;
            }

            r_IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
