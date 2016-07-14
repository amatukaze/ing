using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public enum NetworkSessionType { Normal, Resource, API }
    public enum NetworkSessionState { Requested, Responsed, Cached, LoadedFromCache, Error }

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

        public string RequestBodyString { get; internal set; }

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

        string r_ResponseBodyString;
        public string ResponseBodyString
        {
            get { return r_ResponseBodyString; }
            internal set
            {
                if (r_ResponseBodyString != value)
                {
                    r_ResponseBodyString = value;
                    OnPropertyChanged(nameof(ResponseBodyString));
                }
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
