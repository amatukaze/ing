using System;

namespace Sakuno.ING.Game
{
    internal interface IUpdatable<TId, TRaw> : IBindable, IIdentifiable<TId>
        where TId : struct
    {
        DateTimeOffset UpdationTime { get; }
        void Update(TRaw raw, DateTimeOffset timeStamp);
    }
}
