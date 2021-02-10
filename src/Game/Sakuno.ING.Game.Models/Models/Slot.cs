namespace Sakuno.ING.Game.Models
{
    public abstract class Slot : BindableObject
    {
        public abstract SlotItem? Item { get; }

        private ClampedValue _planeCount;
        public ClampedValue PlaneCount
        {
            get => _planeCount;
            internal set => Set(ref _planeCount, value);
        }
    }
}
