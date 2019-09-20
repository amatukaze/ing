using System.Diagnostics;

namespace Sakuno.ING.Game
{
    internal class IdTableDebuggerProxy<TId, T, TRaw, TOwner>
        where TId : struct
        where T : class, IUpdatable<TId, TRaw>
        where TRaw : IIdentifiable<TId>
    {
        private readonly IdTable<TId, T, TRaw, TOwner> origin;

        public IdTableDebuggerProxy(IdTable<TId, T, TRaw, TOwner> origin)
        {
            this.origin = origin;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Item => origin.list.ToArray();
    }
}
