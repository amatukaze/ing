namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class RawDataWrapper<T> : ModelBase
    {
        internal T RawData { get; private set; }

        protected RawDataWrapper(T rpRawData)
        {
            RawData = rpRawData;
        }

        public void Update(T rpRawData)
        {
            RawData = rpRawData;
            OnRawDataUpdated();
        }
        protected virtual void OnRawDataUpdated()
        {
        }
    }
}
