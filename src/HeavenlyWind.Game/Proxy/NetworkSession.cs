using System;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public enum NetworkSessionType { Normal, Resource, API }
    public enum NetworkSessionState { Requested, Responsed, Cached, LoadedFromCache, Error, Blocked }

    public class NetworkSession : ModelBase
    {
        public virtual NetworkSessionType Type => NetworkSessionType.Normal;

        public DateTimeOffset Time { get; } = DateTimeOffset.Now;

        public string FullUrl { get; }
        public virtual string DisplayUrl => FullUrl;

        NetworkSessionState r_State;
        public NetworkSessionState State
        {
            get { return r_State; }
            internal set
            {
                r_State = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public string Method { get; internal set; }

        public virtual string RequestBodyString { get; internal set; }

        int? r_ContentLength;
        public int? ContentLength
        {
            get { return r_ContentLength; }
            internal set
            {
                r_ContentLength = value;
                OnPropertyChanged(nameof(ContentLength));
            }
        }
        int r_LoadedBytes;
        public int LoadedBytes
        {
            get { return r_LoadedBytes; }
            internal set
            {
                r_LoadedBytes = value;
                OnPropertyChanged(nameof(LoadedBytes));
            }
        }

        int r_StatusCode;
        public int StatusCode
        {
            get { return r_StatusCode; }
            internal set
            {
                r_StatusCode = value;
                OnPropertyChanged(nameof(StatusCode));
            }
        }

        public byte[] ResponseBody { get; internal set; }

        WeakReference<string> _responseBodyString;
        public string ResponseBodyString
        {
            get
            {
                if (_responseBodyString == null || !_responseBodyString.TryGetTarget(out var result))
                {
                    result = Encoding.UTF8.GetString(ResponseBody);

                    if (_responseBodyString == null)
                        _responseBodyString = new WeakReference<string>(result);
                    else
                        _responseBodyString.SetTarget(result);
                }

                return result;
            }
        }

        public SessionHeader[] RequestHeaders { get; internal set; }
        SessionHeader[] r_ResponseHeaders;
        public SessionHeader[] ResponseHeaders
        {
            get { return r_ResponseHeaders; }
            internal set
            {
                if (r_ResponseHeaders != value)
                {
                    r_ResponseHeaders = value;
                    OnPropertyChanged(nameof(ResponseHeaders));
                }
            }
        }

        string r_ErrorMessage;
        public string ErrorMessage
        {
            get { return r_ErrorMessage; }
            internal set
            {
                r_ErrorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        internal NetworkSession(string rpFullUrl)
        {
            FullUrl = rpFullUrl;
        }
    }
}
