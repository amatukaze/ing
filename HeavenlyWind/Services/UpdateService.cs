using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class UpdateService : ModelBase
    {
        public static UpdateService Instance { get; } = new UpdateService();

        string[] r_FilesToBeChecked;

        public CheckForUpdateResult.UpdateInfo Info { get; private set; }

        public ICommand DownloadCommand { get; }

        UpdateService()
        {
            DownloadCommand = new DelegatedCommand(() => Process.Start(Info?.Link));

            var rRootPath = Path.GetDirectoryName(GetType().Assembly.Location);
            var rRootPathLength = rRootPath.Length + 1;

            IEnumerable<string> rDataFiles;
            var rDataDirectory = new DirectoryInfo(Path.Combine(rRootPath, "Data"));
            if (rDataDirectory.Exists)
                rDataFiles = rDataDirectory.EnumerateFiles("*").Select(r => r.FullName.Substring(rRootPathLength));
            else
                rDataFiles = new string[]
                {
                    QuestProgressService.DataFilename,
                    MapService.DataFilename,
                    ShipLockingService.DataFilename,
                };

            var rSRDirectory = new DirectoryInfo(Path.Combine(rRootPath, "Resources", "Strings"));
            var rSRFiles = rSRDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Select(r => r.FullName.Substring(rRootPathLength));

            r_FilesToBeChecked = rDataFiles.Concat(rSRFiles).ToArray();
        }

        internal async void CheckForUpdate()
        {
            if (Environment.GetCommandLineArgs().Any(r => r.OICEquals("--no-check-update")) || !Preference.Current.CheckUpdate || !NetworkInterface.GetIsNetworkAvailable())
                return;

            try
            {
                await CheckForUpdateCore();
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_CheckForUpdate_Exception, e.Message));
            }
        }

        async Task CheckForUpdateCore()
        {
            var rRequest = WebRequest.CreateHttp("https://api.sakuno.moe/ing/check_for_update");
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

                if (rResult.Files != null)
                    foreach (var rFileUpdate in rResult.Files)
                    {
                        var rFile = new FileInfo(rFileUpdate.Name);

                        switch (rFileUpdate.Action)
                        {
                            case CheckForUpdateFileAction.CreateOrOverwrite:
                                EnsureDirectory(rFile);

                                using (var rWriter = new StreamWriter(rFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read)))
                                    await rWriter.WriteAsync(rFileUpdate.Content);

                                rFile.LastWriteTime = DateTimeUtil.FromUnixTime((ulong)rFileUpdate.Timestamp).LocalDateTime;
                                break;

                            case CheckForUpdateFileAction.Delete:
                                NativeMethods.Kernel32.DeleteFileW(rFile.FullName);
                                break;

                            case CheckForUpdateFileAction.Rename:
                                EnsureDirectory(rFile);

                                rFile.MoveTo(rFileUpdate.Content);
                                break;
                        }
                    }
            }
        }
        void EnsureDirectory(FileInfo rpFile)
        {
            var rDirectory = rpFile.Directory;
            if (!rDirectory.Exists)
                rDirectory.Create();
        }
    }
}
