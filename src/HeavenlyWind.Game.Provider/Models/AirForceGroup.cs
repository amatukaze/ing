using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class AirForceGroup : BindableObject, IIdentifiable, IBindableWithChildren<AirForceSquadron>
    {
        protected AirForceGroup(int id) => Id = id;

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

        private int _distance;
        public int Distance
        {
            get => _distance;
            protected set
            {
                if (_distance != value)
                {
                    _distance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private AirForceGroupAction _action;
        public AirForceGroupAction Action
        {
            get => _action;
            protected set
            {
                if (_action != value)
                {
                    _action = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IBindableCollection<AirForceSquadron> _squadrons;
        public IBindableCollection<AirForceSquadron> Squadrons
        {
            get => _squadrons;
            protected set
            {
                if (_squadrons != value)
                {
                    _squadrons = value;
                    ChildrenChanged?.Invoke(value);
                    NotifyPropertyChanged();
                }
            }
        }

        IBindableCollection<AirForceSquadron> IBindableWithChildren<AirForceSquadron>.Children => Squadrons;
        public event Action<IBindableCollection<AirForceSquadron>> ChildrenChanged;
    }

    public enum AirForceGroupAction
    {
        Idle = 0,
        Sortie = 1,
        Defence = 2,
        Retreat = 3,
        Rest = 4,
    }
}
