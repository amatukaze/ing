using System.Diagnostics;

namespace Sakuno.ING.Game
{
    internal class IdTableDebuggerProxy<TId, T, TRaw, TOwner>
        where TId : struct
        where T : class, IUpdatable<TId, TRaw>
        where TRaw : IIdentifiable<TId>
    {
        private readonly IdTable<TId, T, TRaw, TOwner> _origin;

        public IdTableDebuggerProxy(IdTable<TId, T, TRaw, TOwner> origin)
        {
            _origin = origin;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Item => _origin._list.ToArray();
    }
}
