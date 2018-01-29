namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class AirForceSquadron : Slot
    {
        private AirForceSquadronState _state;
        public AirForceSquadronState State
        {
            get => _state;
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private AirForceSquadronCondition _condition;
        public AirForceSquadronCondition Condition
        {
            get => _condition;
            protected set
            {
                if (_condition != value)
                {
                    _condition = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    public enum AirForceSquadronState
    {
        Empty = 0,
        Ready = 1,
        Switching = 2,
    }

    public enum AirForceSquadronCondition
    {
        None = 0,
        Ready = 1,
        Yellow = 2,
        Red = 3,
    }
}
