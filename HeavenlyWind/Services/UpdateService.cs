using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.Serialization.MessagePack;
using Sakuno.SystemInterop;
using Sakuno.UserInterface.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class UpdateService : ModelBase
    {
        public static UpdateService Instance { get; } = new UpdateService();

        static Regex r_ShipAvatarRegex = new Regex(@"^(-?\d+)_([nd])\.png$", RegexOptions.Compiled);

        public CheckForUpdateResult.UpdateInfo Info { get; private set; }

        public ICommand DownloadCommand { get; }

        public ICommand HideNotificationCommand { get; }

        UpdateService()
        {
            DownloadCommand = new DelegatedCommand(() => Process.Start(Info?.Link));

            HideNotificationCommand = new DelegatedCommand<UpdateNotificationMode>(HideNotification);

            DataStore.Updated += rpName =>
            {
                switch (rpName)
                {
                    case "ship_avatar":
                        UpdateShipAvatars();
                        break;
                }
            };
        }

        internal void CheckForUpdate()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            var rFilesToBeChecked = GetFileList().ToArray();

            try
            {
                for (var i = 1; i <= 4; i++)
                    try
                    {
                        CheckForUpdateCore(rFilesToBeChecked);

                        return;
                    }
                    catch (WebException)
                    {
                        if (i == 4)
                            throw;

                        Task.Delay(i * 4000).Wait();
                    }

                if (!Directory.EnumerateFiles(Path.Combine(ProductInfo.RootDirectory, "Resources", "Avatars", "Ships"), "*.png").Any())
                    UpdateShipAvatars();
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_CheckForUpdate_Exception, e.Message));
            }
        }
        IEnumerable<string> GetFileList()
        {
            var rRootPath = ProductInfo.RootDirectory;

            var rResourcesDirectory = new DirectoryInfo(Path.Combine(rRootPath, "Resources"));
            if (!rResourcesDirectory.Exists)
                yield break;

            var rRootPathLength = rRootPath.Length + 1;

            foreach (var rFile in rResourcesDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var rExtension = rFile.Extension;
                if (rExtension.OICEndsWith("json") || rExtension.OICEndsWith("xml"))
                    yield return rFile.FullName.Substring(rRootPathLength);
            }
        }
        void CheckForUpdateCore(string[] rpFileList)
        {
            using (var rResponse = CreateRequest(rpFileList).GetResponse())
            using (var rStream = rResponse.GetResponseStream())
            {
                var rReader = new JsonTextReader(new StreamReader(rStream));
                var rResult = JObject.Load(rReader).ToObject<CheckForUpdateResult>();

                Info = rResult.Update;

                if (Info.IsAvailable)
                    switch (Preference.Instance.Update.NotificationMode.Value)
                    {
                        case UpdateNotificationMode.Disabled:
                            Info.IsAvailable = false;
                            break;

                        case UpdateNotificationMode.IgnoreOptionalUpdate:
                            var rCurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                            var rLastestVersion = new Version(Info.Version);
                            if (!Info.IsImportantUpdate &&
                                rCurrentVersion.Major == rLastestVersion.Major &&
                                rCurrentVersion.Minor == rLastestVersion.Minor &&
                                rCurrentVersion.Build == rLastestVersion.Build &&
                                rCurrentVersion.Revision < rLastestVersion.Revision)
                                Info.IsAvailable = false;
                            break;
                    }

                OnPropertyChanged(nameof(Info));

                ProcessFiles(rResult);
            }
        }
        HttpWebRequest CreateRequest(string[] rpFileList)
        {
            var rRequest = WebRequest.CreateHttp("http://heavenlywind.cc/api/check_for_update");
            rRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            rRequest.UserAgent = ProductInfo.UserAgent;
            rRequest.Method = "POST";

            var rData = new
            {
                client = new
                {
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    fw = OS.DotNetFrameworkReleaseNumber,
                },

                data = GetOfficialDataStoreItem().Select(r => new
                {
                    name = r.Name,
                    timestamp = r.Timestamp?.ToUnixTime(),
                }).ToArray(),

                files = rpFileList.Select(r =>
                {
                    var rFile = new FileInfo(r);

                    return new
                    {
                        name = r,
                        timestamp = rFile.Exists ? (long?)new DateTimeOffset(rFile.LastWriteTime).ToUnixTime() : null,
                    };
                }).ToArray(),
            };

            using (var rStream = rRequest.GetRequestStream())
            {
                var rSerializer = new JsonSerializer();
                var rWriter = new JsonTextWriter(new StreamWriter(rStream));

                rSerializer.Serialize(rWriter, rData);
                rWriter.Flush();
            }

            return rRequest;
        }
        IEnumerable<DataStoreItem> GetOfficialDataStoreItem()
        {
            DataStoreItem rItem;

            if (DataStore.TryGet("map_node", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("quest", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("expedition", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("ship_locking", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("abyssal_ship_plane", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("anti_air_cut_in", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("equipment_icon", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;

            if (DataStore.TryGet("ship_avatar", DataStoreRetrieveOption.ExcludeContent, out rItem))
                yield return rItem;
        }
        void ProcessFiles(CheckForUpdateResult rpResult)
        {
            if (rpResult.Files != null)
                foreach (var rFileUpdate in rpResult.Files)
                {
                    var rFile = new FileInfo(rFileUpdate.Name);

                    switch (rFileUpdate.Action)
                    {
                        case CheckForUpdateFileAction.CreateOrOverwrite:
                            EnsureDirectory(rFile);

                            using (var rWriter = new StreamWriter(rFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read)))
                                rWriter.Write(rFileUpdate.Content);

                            rFile.LastWriteTime = DateTimeUtil.FromUnixTime(rFileUpdate.Timestamp).LocalDateTime;
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

            if (rpResult.Data != null)
                foreach (var rData in rpResult.Data)
                    if (rData.Content != null)
                        DataStore.Set(rData.Name, rData.Content, rData.Timestamp);
                    else
                        DataStore.Delete(rData.Name);
        }
        void EnsureDirectory(FileInfo rpFile)
        {
            var rDirectory = rpFile.Directory;
            if (!rDirectory.Exists)
                rDirectory.Create();
        }

        void HideNotification(UpdateNotificationMode rpMode)
        {
            if (Info == null)
                return;

            Preference.Instance.Update.NotificationMode.Value = rpMode;
            Info.IsAvailable = false;
            OnPropertyChanged(nameof(Info));
        }

        async void UpdateShipAvatars()
        {
            try
            {
                var rRequest = WebRequest.CreateHttp("http://heavenlywind.cc/api/ship_avatars");
                rRequest.UserAgent = ProductInfo.UserAgent;

                var rPath = Path.Combine(ProductInfo.RootDirectory, "Resources", "Avatars", "Ships") + "\\";
                var rDirectory = new DirectoryInfo(rPath);
                if (!rDirectory.Exists)
                    rDirectory.Create();

                var rFiles = Directory.EnumerateFiles(rPath, "*.png").ToArray();
                if (rFiles.Length == 0)
                    rRequest.Method = "GET";
                else
                {
                    rRequest.Method = "POST";

                    var rData = new SortedList<int, int>(rFiles.Length);

                    foreach (var rFile in rFiles)
                    {
                        var rMatch = r_ShipAvatarRegex.Match(Path.GetFileName(rFile));
                        if (!rMatch.Success)
                            continue;

                        var rShip = int.Parse(rMatch.Groups[1].Value);

                        int rType;
                        rData.TryGetValue(rShip, out rType);

                        rType += rMatch.Groups[2].Value == "n" ? 1 : 2;

                        rData[rShip] = rType;
                    }

                    using (var rStream = await rRequest.GetRequestStreamAsync())
                    {
                        var rSerializer = new JsonSerializer();
                        var rWriter = new JsonTextWriter(new StreamWriter(rStream));

                        rSerializer.Serialize(rWriter, rData);
                        await rWriter.FlushAsync();
                    }
                }

                using (var rResponse = await rRequest.GetResponseAsync())
                using (var rStream = rResponse.GetResponseStream())
                {
                    var rResult = (IDictionary<object, object>)MessagePack.Unpack(new MessagePackReader(rStream));

                    foreach (var rInfo in rResult)
                    {
                        var rData = rInfo.Value as object[];
                        if (rData == null || rData.Length == 0)
                            continue;

                        var rShip = rInfo.Key.ToString();

                        if (rData[0] != null)
                            File.WriteAllBytes(rPath + rShip + "_n.png", (byte[])rData[0]);

                        if (rData.Length > 1 && rData[1] != null)
                            File.WriteAllBytes(rPath + rShip + "_d.png", (byte[])rData[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_CheckForUpdate_Exception, e.Message));
            }
        }
    }
}
