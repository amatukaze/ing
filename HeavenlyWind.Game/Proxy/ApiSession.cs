using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class ApiSession : NetworkSession
    {
        public override NetworkSessionType Type => NetworkSessionType.API;

        string r_API;
        public override string DisplayUrl => r_API;

        internal ApiSession(string rpFullUrl) : base(rpFullUrl)
        {
            var rPosition = rpFullUrl.IndexOf("/kcsapi/");
            if (rPosition == -1)
                throw new ArgumentException(nameof(rpFullUrl));

            r_API = rpFullUrl.Substring(rPosition + 8);
        }
    }
}
