namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipSlot : ModelBase
    {
        public Equipment Equipment { get; private set; }
        public bool HasEquipment => Equipment != null && Equipment != Equipment.Dummy;

        public int MaxPlaneCount { get; set; }

        int r_PlaneCount;
        public int PlaneCount
        {
            get { return r_PlaneCount; }
            internal set
            {
                if (r_PlaneCount != value)
                {
                    r_PlaneCount = value;
                    OnPropertyChanged(nameof(PlaneCount));
                    OnPropertyChanged(nameof(HasLostPlane));
                }
            }
        }
        public bool HasLostPlane => r_PlaneCount != MaxPlaneCount;

        internal ShipSlot(Equipment rpEquipment, int rpMaxPlaneCount, int rpPlaneCount)
        {
            Equipment = rpEquipment;
            PlaneCount = rpPlaneCount;
            MaxPlaneCount = rpMaxPlaneCount;
        }

        public override string ToString() => $"Equipment = \"{Equipment.FullName}\", PlaneCount = {PlaneCount}/{MaxPlaneCount}";
    }
}