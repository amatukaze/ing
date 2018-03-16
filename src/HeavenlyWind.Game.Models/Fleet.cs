using System;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class Fleet : BindableObject, IIdentifiable, IBindableWithChildren<Ship>
    {
        protected Fleet(int id) => Id = id;

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

        private IBindableCollection<Ship> _ships;
        public IBindableCollection<Ship> Ships
        {
            get => _ships;
            protected set
            {
                if (_ships != value)
                {
                    _ships = value;
                    ChildrenChanged?.Invoke(value);
                    NotifyPropertyChanged();
                }
            }
        }

        private FleetExpeditionState _expeditionState;
        public FleetExpeditionState ExpeditionState
        {
            get => _expeditionState;
            protected set
            {
                if (_expeditionState != value)
                {
                    _expeditionState = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ExpeditionInfo _expedition;
        public ExpeditionInfo Expedition
        {
            get => _expedition;
            protected set
            {
                if (_expedition != value)
                {
                    _expedition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTimeOffset _expeditionCompletionTime;
        public DateTimeOffset ExpeditionCompletionTime
        {
            get => _expeditionCompletionTime;
            set
            {
                if (_expeditionCompletionTime != value)
                {
                    _expeditionCompletionTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        IBindableCollection<Ship> IBindableWithChildren<Ship>.Children => Ships;
        public event Action<IBindableCollection<Ship>> ChildrenChanged;
    }
}
