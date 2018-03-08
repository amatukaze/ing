namespace Sakuno.KanColle.Amatsukaze.Game
{
    public abstract class Admiral : BindableObject, IIdentifiable
    {
        protected Admiral(int id) => Id = id;

        public int Id { get; }

        private string _name;
        public string Name
        {
            get => _name;
            protected set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _signature;
        public string Signature
        {
            get => _signature;
            protected set
            {
                if (_signature != value)
                {
                    _signature = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Leveling _leveling;
        public Leveling Leveling
        {
            get => _leveling;
            protected set
            {
                _leveling = value;
                NotifyPropertyChanged();
            }
        }

        private AdmiralRank _rank;
        public AdmiralRank Rank
        {
            get => _rank;
            set
            {
                if (_rank != value)
                {
                    _rank = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
