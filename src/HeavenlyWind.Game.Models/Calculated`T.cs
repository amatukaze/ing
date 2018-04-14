namespace Sakuno.KanColle.Amatsukaze.Game
{
    public abstract class Calculated<TRaw> : BindableObject, IIdentifiable
        where TRaw : IIdentifiable<int>
    {
        public int Id { get; }
        internal bool UpdateFlag;

        protected Calculated(TRaw raw)
        {
            Id = raw.Id;
            Update(raw);
        }

        public abstract void Update(TRaw raw);
    }
}
