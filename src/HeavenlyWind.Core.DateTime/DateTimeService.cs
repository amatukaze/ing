using Sakuno.Net;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace Sakuno.KanColle.Amatsukaze.Services.DateTime
{
    public class DateTimeService : IDateTimeService
    {
        const int NDPPort = 123;
        const int JapanTimeZoneOffset = 9;

        TimeSpan _jstOffset = TimeSpan.FromHours(JapanTimeZoneOffset);

        public bool IsSycned { get; private set; }

        long _baseTimestamp;
        DateTimeOffset _baseTime;

        long _initialTimestamp;
        DateTimeOffset _now;

        static long _tickFrequency;

        public DateTimeOffset Now
        {
            get
            {
                if (!IsSycned)
                    return DateTimeOffset.Now;

                return _now;
            }
        }

        static PropertyChangedEventArgs _allPropertiesChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        static DateTimeService()
        {
            _allPropertiesChanged = EventArgsCache.PropertyChanged.Get(string.Empty);

            _tickFrequency = DateTimeUtil.TicksPerSecond / Stopwatch.Frequency;
        }

        async void SetTimer()
        {
            var frequency = (double)Stopwatch.Frequency;

            while (true)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var diff = timestamp - _initialTimestamp;
                var increment = diff * _tickFrequency;

                _now = _baseTime.AddSeconds(diff / frequency);

                PropertyChanged?.Invoke(this, _allPropertiesChanged);

                await Task.Delay(1000);
            }
        }

        public async void SyncDateTime()
        {
            var hostnames = new[]
            {
                "jp.pool.ntp.org",
                "pool.ntp.org",
                "us.pool.ntp.org",
                "cn.pool.ntp.org",
                "jp.ntp.org.cn",
                "us.ntp.org.cn",
                "cn.ntp.org.cn",
            };

            foreach (var hostname in hostnames)
            {
                var task = SynchronizeAsync(hostname);

                if (await Task.WhenAny(task, Task.Delay(3000)) != task)
                    continue;

                SetTimer();

                IsSycned = true;
                Console.WriteLine("Date & time synchronized.");
                return;
            }
        }

        async Task SynchronizeAsync(string hostname)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var addresses = await Dns.GetHostAddressesAsync(hostname);
                var ipEndPoint = new IPEndPoint(addresses[0], NDPPort);

                var context = new SocketAsyncOperationContext() { RemoteEndPoint = ipEndPoint };

                var buffer = new byte[48];
                buffer[0] = 0x1B;

                context.SetBuffer(buffer);

                await socket.SendToAsync(context);
                await socket.ReceiveFromAsync(context);

                var integer = (ulong)buffer[40] << 24 | (ulong)buffer[41] << 16 | (ulong)buffer[42] << 8 | buffer[43];
                var fraction = (ulong)buffer[44] << 24 | (ulong)buffer[45] << 16 | (ulong)buffer[46] << 8 | buffer[47];
                var milliseconds = (long)(integer * 1000 + (fraction >> 20));

                const long TicksOf20thCentury = 599266080000000000;
                const long TicksPerMillisecond = 10000;

                _baseTimestamp = TicksOf20thCentury + milliseconds * TicksPerMillisecond;
                _baseTime = new DateTimeOffset(_baseTimestamp, TimeSpan.Zero).ToOffset(_jstOffset);
                _initialTimestamp = Stopwatch.GetTimestamp();
            }
        }
    }
}
