using Sakuno.Collections;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class ApiSession : NetworkSession
    {
        static char[] r_RequestSeparator = new[] { '&' };

        public override NetworkSessionType Type => NetworkSessionType.API;

        string r_API;
        public override string DisplayUrl => r_API;

        public override string RequestBodyString
        {
            get { return base.RequestBodyString; }

            internal set
            {
                base.RequestBodyString = value;

                if (!value.IsNullOrEmpty())
                {
                    Parameters = new ListDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var rParameter in value.Split(r_RequestSeparator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var rPosition = rParameter.IndexOf('=');
                        if (rPosition == -1)
                            continue;

                        Parameters.Add(rParameter.Remove(rPosition), rParameter.Substring(rPosition + 1));
                    }
                }
            }
        }

        public IDictionary<string, string> Parameters { get; private set; }

        internal ApiSession(string rpFullUrl) : base(rpFullUrl)
        {
            var rPosition = rpFullUrl.IndexOf("/kcsapi/");
            if (rPosition == -1)
                throw new ArgumentException(nameof(rpFullUrl));

            r_API = rpFullUrl.Substring(rPosition + 8);
        }
    }
}
