namespace Sakuno.KanColle.Amatsukaze.Game
{
    public abstract class Calculated<TRaw> : BindableObject, IIdentifiable
        where TRaw : IIdentifiable<int>
    {
        public int Id { get; }
        protected ITableProvider Owner { get; }
        internal bool UpdateFlag;

        protected Calculated(TRaw raw, ITableProvider owner)
        {
            Id = raw.Id;
            Owner = owner;
            Update(raw);
        }

        public abstract void Update(TRaw raw);
    }
}
