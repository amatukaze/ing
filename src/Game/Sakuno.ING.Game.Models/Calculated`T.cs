using System;

namespace Sakuno.ING.Game
{
    public abstract class Calculated<TId, TRaw> : BindableObject, IIdentifiable<TId>
        where TId : struct, IComparable<TId>
    {
        public TId Id { get; }

        public DateTimeOffset UpdationTime { get; protected set; }

        private protected Calculated(TId id)
        {
            Id = id;
        }

        public abstract void Update(TRaw raw, DateTimeOffset timeStamp);
    }
}
