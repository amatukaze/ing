using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetTotalStatusItem : ModelBase
    {
        public FleetTotalStatusKind Kind { get; }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged();
            }
        }

        public FleetTotalStatusItem(FleetTotalStatusKind kind)
        {
            Kind = kind;
        }
    }
}
