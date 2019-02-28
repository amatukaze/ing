namespace Sakuno.ING.Game.Models
{
    public class HomeportSlot : Slot
    {
        private Equipment _equipment;
        public new Equipment Equipment
        {
            get => _equipment;
            set
            {
                _equipment = value;
                using (EnterBatchNotifyScope())
                {
                    ImprovementLevel = value?.ImprovementLevel ?? 0;
                    NotifyPropertyChanged(nameof(ImprovementLevel));
                    AirProficiency = value?.AirProficiency ?? 0;
                    NotifyPropertyChanged(nameof(AirProficiency));
                    base.Equipment = value?.Info;
                }
            }
        }
    }
}
