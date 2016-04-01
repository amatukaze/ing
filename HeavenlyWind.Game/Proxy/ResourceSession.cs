using System;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class ResourceSession : NetworkSession
    {
        static Regex r_VersionRegex = new Regex(@"(?<=\?version=).+?(?=(?:$|&))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        string r_DisplayUrl;
        public override string DisplayUrl => r_DisplayUrl;

        public string Path { get; }

        public string CacheFilename { get; internal set; }
        public string CacheVersion { get; }

        internal ResourceSession(string rpFullUrl, string rpPath) : base(rpFullUrl)
        {
            r_DisplayUrl = rpPath.Substring(1);

            Uri rUri;
            if (Uri.TryCreate(rpFullUrl, UriKind.Absolute, out rUri))
                Path = rUri.AbsolutePath;

            var rVersionMatch = r_VersionRegex.Match(rpFullUrl);
            if (rVersionMatch.Success)
                CacheVersion = rVersionMatch.Value;
        }
    }
}
