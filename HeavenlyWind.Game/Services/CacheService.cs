using Fiddler;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class CacheService
    {
        static Regex r_ExtensionRegex = new Regex(@"\.\w+$", RegexOptions.Compiled);

        public static CacheService Instance { get; } = new CacheService();

        public CacheMode CurrentMode = Preference.Current.Cache.Mode;

        public string CacheDirectory = Preference.Current.Cache.Path;

        CacheService() { }

        internal void ProcessRequest(ResourceSession rpResourceSession, Session rpSession)
        {
            if (CurrentMode == CacheMode.Disabled)
                return;

            string rFilename;
            var rNoVerification = CheckFileinCache(rpSession, out rFilename);

            rpResourceSession.CacheFilename = rFilename;

            if (rNoVerification == null)
                return;

            if (rNoVerification.Value)
                LoadFile(rFilename, rpResourceSession, rpSession);
            else
            {
                rpSession.oRequest["If-Modified-Since"] = File.GetLastWriteTime(rFilename).ToString("R");
                rpSession.bBufferResponse = true;
            }
        }
        bool? CheckFileinCache(Session rpSession, out string ropFilename)
        {
            ropFilename = null;

            Uri rUri;
            if (!Uri.TryCreate(rpSession.fullUrl, UriKind.Absolute, out rUri))
                return null;

            var rFilename = CacheDirectory + rUri.AbsolutePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
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

        internal void ProcessResponse(ResourceSession rpResourceSession, Session rpSession)
        {
            if (rpSession.responseCode != 304 || CurrentMode != CacheMode.VerifyVersion)
                return;

            LoadFile(rpResourceSession.CacheFilename, rpResourceSession, rpSession);

            rpResourceSession.State = NetworkSessionState.LoadedFromCache;
        }

        void LoadFile(string rpFilename, ResourceSession rpResourceSession, Session rpSession)
        {
            if (!rpSession.bBufferResponse)
                rpSession.utilCreateResponseAndBypassServer();

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

            rpResourceSession.State = NetworkSessionState.LoadedFromCache;
        }

        internal void ProcessOnCompletion(ResourceSession rpResourceSession, Session rpSession)
        {
            if (rpSession.responseCode != 200 || rpResourceSession.CacheFilename == null)
                return;

            var rDirectoryName = Path.GetDirectoryName(rpResourceSession.CacheFilename);
            if (!Directory.Exists(rDirectoryName))
                Directory.CreateDirectory(rDirectoryName);

            var rFile = new FileInfo(rpResourceSession.CacheFilename);
            if (rFile.Exists)
                rFile.Delete();

            rpSession.SaveResponseBody(rFile.FullName);
            rFile.LastWriteTime = Convert.ToDateTime(rpSession.oResponse["Last-Modified"]);

            rpResourceSession.State = NetworkSessionState.Cached;
        }
    }
}
