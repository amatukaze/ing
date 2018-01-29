namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Slot : BindableObject
    {
        private Equipment _equipment;
        public Equipment Equipment
        {
            get => _equipment;
            set
            {
                if (_equipment != value)
                {
                    _equipment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ClampedValue _aircraft;
        public ClampedValue AirCraft
        {
            get => _aircraft;
            set
            {
                if (_aircraft != value)
                {
                    _aircraft = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
