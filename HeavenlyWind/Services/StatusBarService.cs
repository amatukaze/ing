namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class StatusBarService : ModelBase
    {
        public static StatusBarService Instance { get; } = new StatusBarService();

        string r_Message;
        public string Message
        {
            get { return r_Message; }
            set
            {
                if (r_Message != value)
                {
                    r_Message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        StatusBarService() { }

        public void Initialize() => Logger.LogAdded += r => Message = $"{r.Time}: {r.Content}";
    }
}
