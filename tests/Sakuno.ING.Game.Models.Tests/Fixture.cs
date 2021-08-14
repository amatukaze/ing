using Sakuno.ING.Messaging;
using System;
using System.Buffers;
using System.IO;
using System.Reactive.Subjects;
using System.Text;

namespace Sakuno.ING.Game.Models.Tests
{
    public class Fixture
    {
        public GameProvider GameProvider { get; }
        public NavalBase NavalBase { get; }

        private readonly Subject<ApiMessage> _subject = new();

        public Fixture()
        {
            GameProvider = new(new SourceWrapper(_subject));
            NavalBase = new(GameProvider);
        }

        public void Process(string api, string responseFilename) => Process(api, null, responseFilename);
        public void Process(string api, string? requestFilename, string? responseFilename)
        {
            var request = string.Empty;

            if (requestFilename is not null)
            {
                var requestResourceName = "Sakuno.ING.Game.Models.Tests.Data." + requestFilename;
                using var requestStream = typeof(Fixture).Assembly.GetManifestResourceStream(requestResourceName)!;

                var builder = new StringBuilder(128);
                var reader = new StreamReader(requestStream);

                while (reader.ReadLine() is { } line)
                {
                    if (builder.Length > 0)
                        builder.Append('&');

                    builder.Append(line);
                }

                request = builder.ToString();
            }

            if (responseFilename is null)
            {
                const string Response = "{\"api_result\":1,\"api_result_msg\":\"成功\",\"api_data\":null}";

                _subject.OnNext(new(api, request.AsMemory(), Encoding.UTF8.GetBytes(Response)));
                return;
            }

            var responseResourceName = "Sakuno.ING.Game.Models.Tests.Data." + responseFilename;
            using var responseStream = typeof(Fixture).Assembly.GetManifestResourceStream(responseResourceName)!;

            const string Prefix = "{\"api_result\":1,\"api_result_msg\":\"成功\",\"api_data\":";

            var prefixLength = Encoding.UTF8.GetByteCount(Prefix);
            var actualLength = prefixLength + (int)responseStream.Length + 1;
            var responseBuffer = ArrayPool<byte>.Shared.Rent(actualLength);

            try
            {
#if NETCOREAPP
                Encoding.UTF8.GetBytes(Prefix, responseBuffer);

                responseStream.Read(responseBuffer.AsSpan(prefixLength));
#else
                Encoding.UTF8.GetBytes(Prefix, 0, Prefix.Length, responseBuffer, 0);

                responseStream.Read(responseBuffer, prefixLength, (int)responseStream.Length);
#endif

                responseBuffer[prefixLength + (int)responseStream.Length] = (byte)'}';

                _subject.OnNext(new(api, request.AsMemory(), responseBuffer.AsMemory(0, actualLength)));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(responseBuffer);
            }
        }

        private class SourceWrapper : IApiMessageSource
        {
            public IObservable<ApiMessage> ApiMessageSource { get; }

            public SourceWrapper(IObservable<ApiMessage> source)
            {
                ApiMessageSource = source;
            }
        }
    }
}
