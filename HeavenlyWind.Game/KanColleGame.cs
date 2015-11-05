namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class KanColleGame : ModelBase
    {
        public static KanColleGame Current { get; } = new KanColleGame();

        public MasterInfo MasterInfo { get; } = new MasterInfo();

        public Port Port { get; } = new Port();

        bool r_IsStarted;
        public bool IsStarted
        {
            get { return r_IsStarted; }
            internal set
            {
                if (r_IsStarted != value)
                {
                    r_IsStarted = value;
                    OnPropertyChanged(nameof(IsStarted));
                }
            }
        }

        KanColleGame() { }
    }
}
