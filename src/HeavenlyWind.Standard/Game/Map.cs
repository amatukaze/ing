using Sakuno.KanColle.Amatsukaze.Game.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public abstract class Map : BindableObject, IIdentifiable
    {
        protected Map(int id) => Id = id;

        public int Id { get; }
        public MapInfo Info { get; protected set; }

        private bool _cleared;
        public bool Cleared
        {
            get => _cleared;
            protected set
            {
                if (_cleared != value)
                {
                    _cleared = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ClampedValue _gauge;
        public ClampedValue Gauge
        {
            get => _gauge;
            set
            {
                if (_gauge != value)
                {
                    _gauge = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
