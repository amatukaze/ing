namespace Sakuno.ING.Game.Models
{
    public partial class Slot : BindableObject
    {
        private Equipment _equipment;
        public Equipment Equipment
        {
            get => _equipment;
            internal set
            {
                if (_equipment != value)
                {
                    _equipment = value;
                    IsEmpty = value == null;
                    UpdateCalculations();
                }
            }
        }

        private ClampedValue _aircraft;
        public ClampedValue Aircraft
        {
            get => _aircraft;
            internal set
            {
                if (_aircraft != value)
                {
                    _aircraft = value;
                    UpdateCalculations();
                }
            }
        }

        private bool _isEmpty = true;
        public bool IsEmpty
        {
            get => _isEmpty;
            private set => Set(ref _isEmpty, value);
        }

        private AirFightPower _airFightPower;
        public AirFightPower AirFightPower
        {
            get => _airFightPower;
            private set => Set(ref _airFightPower, value);
        }

        private double _effectiveLoS;
        public double EffectiveLoS
        {
            get => _effectiveLoS;
            private set => Set(ref _effectiveLoS, value);
        }
    }
}
