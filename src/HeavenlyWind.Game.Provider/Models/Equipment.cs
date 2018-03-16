using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class Equipment : BindableObject, IIdentifiable
    {
        protected Equipment(int id) => Id = id;

        public int Id { get; }

        private EquipmentInfo _info;
        public EquipmentInfo Info
        {
            get => _info;
            protected set
            {
                if (_info != value)
                {
                    _info = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _level;
        public int Level
        {
            get => _level;
            protected set
            {
                if (_level != value)
                {
                    _level = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int _proficiency;
        public int Proficiency
        {
            get => _proficiency;
            protected set
            {
                if (_level != value)
                {
                    _level = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
