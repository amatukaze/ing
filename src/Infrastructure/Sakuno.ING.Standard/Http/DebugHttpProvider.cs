using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sakuno.ING.IO;
using Sakuno.ING.Messaging;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Http
{
    public class DebugHttpProvider : IHttpProvider
    {
        private readonly IShellContextService shell;
        private IFolderFacade folder;
        private ReadOnlyMemory<(string api, string request, string response)> list;

        public DebugHttpProvider(IShellContextService shell) => this.shell = shell;

        public async void Send()
        {
            if (folder is null)
            {
                folder = await shell.Capture().PickFolderAsync();
                if (folder is null) return;
                var index = await folder.GetFileAsync("index.csv");
                if (index is null) return;
                using (var stream = await index.OpenReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    var l = new List<(string, string, string)>();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(line)) continue;
                        var s = line.Split(',');
                        if (s.Length != 3) continue;
                        l.Add((s[0], s[1], s[2]));
                    }
                    list = l.ToArray();
                }
            }
            if (list.Length == 0)
                return;
            var (api, req, res) = list.Span[0];
            list = list.Slice(1);

            async ValueTask<string> GetContentAsync(IFileFacade file)
            {
                if (file is null) return null;
                using (var stream = await file.OpenReadAsync())
                using (var reader = new StreamReader(stream))
                    return await reader.ReadToEndAsync();
            }

            var request = await GetContentAsync(await folder.GetFileAsync(req));
            var response = await GetContentAsync(await folder.GetFileAsync(res));
            if (response != null)
                Received?.Invoke(DateTimeOffset.Now, new HttpMessage(api, (request ?? string.Empty).AsMemory(), response.AsMemory()));
        }

        public event TimedMessageHandler<HttpMessage> Received;
    }
}
