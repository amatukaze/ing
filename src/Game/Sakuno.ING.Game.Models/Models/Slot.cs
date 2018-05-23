namespace Sakuno.ING.Game.Models
{
    public class Slot : BindableObject
    {
        private Equipment _equipment;
        public Equipment Equipment
        {
            get => _equipment;
            internal set
            {
                Set(ref _equipment, value);
                IsEmpty = value == null;
            }
        }

        private ClampedValue _aircraft;
        public ClampedValue Aircraft
        {
            get => _aircraft;
            internal set => Set(ref _aircraft, value);
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            private set => Set(ref _isEmpty, value);
        }
    }
}
