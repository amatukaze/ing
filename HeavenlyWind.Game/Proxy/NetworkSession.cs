namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public enum NetworkSessionState { Requested, Responsed, Cached, LoadedFromCache, Error }

    public class NetworkSession : ModelBase
    {
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
