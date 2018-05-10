namespace Sakuno.ING.Game
{
    public abstract class Calculated<TId, TRaw> : BindableObject, IIdentifiable<TId>
    {
        public TId Id { get; }

        protected Calculated(TId id)
        {
            Id = id;
        }

        public abstract void Update(TRaw raw);
    }
}
