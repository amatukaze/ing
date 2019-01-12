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
                    using (EnterBatchNotifyScope())
                    {
                        bool isEmptyChanged = _equipment is null || value is null;
                        _equipment = value;
                        if (isEmptyChanged)
                            NotifyPropertyChanged(nameof(IsEmpty));
                        NotifyPropertyChanged();
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
                    using (EnterBatchNotifyScope())
                    {
                        _aircraft = value;
                        NotifyPropertyChanged();
                        UpdateCalculations();
                    }
            }
        }

        public bool IsEmpty => Equipment is null;

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
