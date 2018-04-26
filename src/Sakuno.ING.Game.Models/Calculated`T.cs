namespace Sakuno.ING.Game
{
    public abstract class Calculated<TRaw> : BindableObject, IIdentifiable
    {
        public int Id { get; }
        protected ITableProvider Owner { get; }
        internal bool UpdateFlag;

        protected Calculated(int id, ITableProvider owner)
        {
            Id = id;
            Owner = owner;
        }

        public abstract void Update(TRaw raw);
    }
}
