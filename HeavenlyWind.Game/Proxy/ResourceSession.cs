namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class ResourceSession : NetworkSession
    {
        string r_DisplayUrl;
        public override string DisplayUrl => r_DisplayUrl;

        public string CacheFilename { get; internal set; }

        internal ResourceSession(string rpFullUrl, string rpPath) : base(rpFullUrl)
        {
            r_DisplayUrl = rpPath.Substring(1);
        }
    }
}
