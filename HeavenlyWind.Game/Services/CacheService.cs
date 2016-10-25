using Fiddler;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class CacheService
    {
        static Regex r_ExtensionRegex = new Regex(@"\.\w+$", RegexOptions.Compiled);

        public static CacheService Instance { get; } = new CacheService();

        public CacheMode CurrentMode => Preference.Instance.Cache.Mode;
        public string CacheDirectory => Preference.Instance.Cache.Path;

        static object r_ThreadSyncObject = new object();

        SQLiteConnection r_Connection;

        CacheService() { }

        public void Initialize()
        {
            CleanupOutdatedJournal();

            using (var rConnection = new SQLiteConnection(@"Data Source=Data\Cache.db; Page Size=8192").OpenAndReturn())
            using (var rCommand = rConnection.CreateCommand())
            {
                rCommand.CommandText =
                    "PRAGMA journal_mode = WAL; " +

                    "CREATE TABLE IF NOT EXISTS file(" +
                        "name TEXT PRIMARY KEY NOT NULL, " +
                        "version TEXT, " +
                        "timestamp INTEGER NOT NULL) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }

            r_Connection = CoreDatabase.Connection;
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @filename AS cache;";
                rCommand.Parameters.AddWithValue("@filename", new FileInfo(@"Data\Cache.db").FullName);

                rCommand.ExecuteNonQuery();
            }
        }
        void CleanupOutdatedJournal()
        {
            var rDatabaseFile = new FileInfo(@"Data\Cache.db");
            var rSharedMemoryFile = new FileInfo(@"Data\Cache.db-shm");
            var rWALFile = new FileInfo(@"Data\Cache.db-wal");

            var rDatabaseLastFileWriteTime = rDatabaseFile.LastWriteTimeUtc;
            var rCleanup = (rSharedMemoryFile.Exists && rSharedMemoryFile.LastWriteTimeUtc < rDatabaseLastFileWriteTime) ||
                (rWALFile.Exists && rWALFile.LastWriteTimeUtc < rDatabaseLastFileWriteTime);

            if (rCleanup)
            {
                if (rSharedMemoryFile.Exists)
                    rSharedMemoryFile.Delete();

                if (rWALFile.Exists)
                    rWALFile.Delete();
            }
        }

        internal void ProcessRequest(ResourceSession rpResourceSession, Session rpSession)
        {
            if (CurrentMode == CacheMode.Disabled)
                return;

            string rFilename;
            var rNoVerification = CheckFileInCache(rpResourceSession.Path, out rFilename);

            rpResourceSession.CacheFilename = rFilename;

            if (rNoVerification == null)
                return;

            if (!rNoVerification.Value)
            {
                var rTimestamp = new DateTimeOffset(File.GetLastWriteTime(rFilename));

                if (rpResourceSession.Path.OICContains("mainD2.swf") || rpResourceSession.Path.OICContains(".js") || !CheckFileVersionAndTimestamp(rpResourceSession, rTimestamp))
                {
                    rpSession.oRequest["If-Modified-Since"] = rTimestamp.ToString("R");
                    rpSession.bBufferResponse = true;

                    return;
                }
            }

            rpSession.utilCreateResponseAndBypassServer();
            LoadFile(rFilename, rpResourceSession, rpSession);
        }
        bool? CheckFileInCache(string rpPath, out string ropFilename)
        {
            ropFilename = null;

            if (rpPath.IsNullOrEmpty())
                return null;

            var rFilename = CacheDirectory + rpPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rpPath.OICContains("mainD2.swf"))
            {
                ropFilename = rFilename;
                return false;
            }

            var rFilenameForceToLoad = r_ExtensionRegex.Replace(rFilename, ".hack$0");
            if (File.Exists(rFilenameForceToLoad))
            {
                ropFilename = rFilenameForceToLoad;
                return true;
            }

            ropFilename = rFilename;

            if (File.Exists(rFilename))
                return CurrentMode == CacheMode.FullTrust;

            return null;
        }
        bool CheckFileVersionAndTimestamp(ResourceSession rpResourceSession, DateTimeOffset rpTimestamp)
        {
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT (CASE WHEN version IS NOT NULL THEN version ELSE '' END) = @version AND timestamp = @timestamp FROM cache.file WHERE name = @name;";
                rCommand.Parameters.AddWithValue("@name", rpResourceSession.Path);
                rCommand.Parameters.AddWithValue("@version", rpResourceSession.CacheVersion ?? string.Empty);
                rCommand.Parameters.AddWithValue("@timestamp", rpTimestamp.ToUnixTime());

                return Convert.ToBoolean(rCommand.ExecuteScalar());
            }
        }

        internal void ProcessResponse(ResourceSession rpResourceSession, Session rpSession)
        {
            if (rpSession.responseCode != 304 || !rpResourceSession.Path.OICContains("mainD2.swf") && CurrentMode != CacheMode.VerifyVersion)
                return;

            LoadFile(rpResourceSession.CacheFilename, rpResourceSession, rpSession);

            RecordCachedFile(rpResourceSession, File.GetLastWriteTime(rpResourceSession.CacheFilename), false);

            rpResourceSession.State = NetworkSessionState.LoadedFromCache;
        }

        void LoadFile(string rpFilename, ResourceSession rpResourceSession, Session rpSession)
        {
            rpSession.ResponseBody = File.ReadAllBytes(rpFilename);
            rpSession.oResponse["Server"] = "Apache";
            rpSession.oResponse["Connection"] = "close";
            rpSession.oResponse["Accept-Ranges"] = "bytes";
            rpSession.oResponse["Cache-Control"] = "max-age=18000, public";
            rpSession.oResponse["Date"] = DateTime.Now.ToString("R");

            if (rpFilename.EndsWith(".swf", StringComparison.OrdinalIgnoreCase))
                rpSession.oResponse["Content-Type"] = "application/x-shockwave-flash";
            else if (rpFilename.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                rpSession.oResponse["Content-Type"] = "audio/mpeg";
            else if (rpFilename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                rpSession.oResponse["Content-Type"] = "image/png";
            else if (rpFilename.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                rpSession.oResponse["Content-Type"] = "text/css";
            else if (rpFilename.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                rpSession.oResponse["Content-Type"] = "application/x-javascript";

            rpSession.responseCode = 200;

            rpResourceSession.State = NetworkSessionState.LoadedFromCache;
        }

        internal void ProcessOnCompletion(ResourceSession rpResourceSession, Session rpSession)
        {
            if (rpSession.responseCode != 200 || rpResourceSession.State == NetworkSessionState.LoadedFromCache || rpResourceSession.CacheFilename == null ||
                Convert.ToInt32(rpSession.ResponseHeaders["Content-Length"]) != rpSession.ResponseBody.Length)
                return;

            try
            {
                lock (r_ThreadSyncObject)
                {
                    var rLastModified = rpSession.oResponse["Last-Modified"];
                    if (rLastModified.IsNullOrEmpty())
                        return;

                    var rDirectoryName = Path.GetDirectoryName(rpResourceSession.CacheFilename);
                    if (!Directory.Exists(rDirectoryName))
                        Directory.CreateDirectory(rDirectoryName);

                    var rFile = new FileInfo(rpResourceSession.CacheFilename);
                    if (rFile.Exists)
                        rFile.Delete();

                    rpSession.SaveResponseBody(rFile.FullName);

                    var rTimestamp = Convert.ToDateTime(rLastModified);
                    rFile.LastWriteTime = rTimestamp;

                    rpResourceSession.State = NetworkSessionState.Cached;

                    RecordCachedFile(rpResourceSession, rTimestamp, true);
                }
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_Cache_FailedToSaveFile, e.Message));
            }
        }

        void RecordCachedFile(ResourceSession rpResourceSession, DateTime rpTimestamp, bool rpReplace)
        {
            using (var rCommand = r_Connection.CreateCommand())
            {
                if (rpReplace)
                    rCommand.CommandText = "REPLACE INTO cache.file(name, version, timestamp) VALUES(@name, @version, @timestamp);";
                else
                    rCommand.CommandText = "INSERT OR IGNORE INTO cache.file(name, version, timestamp) VALUES(@name, @version, @timestamp);";

                rCommand.Parameters.AddWithValue("@name", rpResourceSession.Path);
                rCommand.Parameters.AddWithValue("@version", rpResourceSession.CacheVersion);
                rCommand.Parameters.AddWithValue("@timestamp", new DateTimeOffset(rpTimestamp).ToUnixTime());

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
