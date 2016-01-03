using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class UpdateService : ModelBase
    {
        public static UpdateService Instance { get; } = new UpdateService();

        string[] r_FilesToBeChecked = new[]
        {
            @"Data\Quests.json",
        };

        public CheckForUpdateResult.UpdateInfo Info { get; private set; }

        UpdateService() { }

        internal void CheckForUpdate()
        {
            if (!Preference.Current.CheckUpdate || !NetworkInterface.GetIsNetworkAvailable())
                return;

            CheckForUpdateCore();
        }

        async void CheckForUpdateCore()
        {
            var rRequest = WebRequest.CreateHttp("https://api.sakuno.moe/kci/check_for_update");
            rRequest.UserAgent = ProductInfo.UserAgent;
            rRequest.Method = "POST";

            var rData = new
            {
                client = new
                {
                    version = ProductInfo.AssemblyVersionString,
                },
                files = r_FilesToBeChecked.Select(r =>
                {
                    var rFile = new FileInfo(r);

                    return new
                    {
                        name = r,
                        timestamp = rFile.Exists ? (long?)new DateTimeOffset(rFile.LastWriteTime).ToUnixTime() : null,
                    };
                }).ToArray(),
            };

            using (var rRequestWriter = new StreamWriter(await rRequest.GetRequestStreamAsync()))
            {
                rRequestWriter.Write(JsonConvert.SerializeObject(rData));
                rRequestWriter.Flush();
            }

            using (var rResponse = await rRequest.GetResponseAsync())
            using (var rReader = new JsonTextReader(new StreamReader(rResponse.GetResponseStream())))
            {
                var rResult = JObject.Load(rReader).ToObject<CheckForUpdateResult>();

                Info = rResult.Update;
                OnPropertyChanged(nameof(Info));

                foreach (var rFileUpdate in rResult.Files)
                {
                    var rFile = new FileInfo(rFileUpdate.Name);

                    switch (rFileUpdate.Action)
                    {
                        case CheckForUpdateFileAction.CreateOrOverwrite:
                            using (var rWriter = new StreamWriter(rFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read)))
                                await rWriter.WriteAsync(rFileUpdate.Content);

                            rFile.LastWriteTime = DateTimeUtil.FromUnixTime((ulong)rFileUpdate.Timestamp).LocalDateTime;
                            break;

                        case CheckForUpdateFileAction.Delete:
                            NativeMethods.Kernel32.DeleteFileW(rFile.FullName);
                            break;
                    }
                }
            }
        }
    }
}
