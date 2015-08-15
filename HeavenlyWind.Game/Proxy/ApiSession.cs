using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class ApiSession : NetworkSession
    {
        string r_DisplayUrl;
        public override string DisplayUrl => r_DisplayUrl;

        public string ResponseBodyString { get; internal set; }

        internal ApiSession(string rpFullUrl) : base(rpFullUrl)
        {
            var rPosition = rpFullUrl.IndexOf("/kcsapi/");
            if (rPosition == -1)
                throw new ArgumentException(nameof(rpFullUrl));

            r_DisplayUrl = rpFullUrl.Substring(rPosition + 8);
        }
    }
}
