using System;

namespace Sakuno.KanColle.Amatsukaze.Collections
{
    class DelegatedProjector<TSource, TDestination> : IProjector<TSource, TDestination>
    {
        Func<TSource, TDestination> _func;

        public DelegatedProjector(Func<TSource, TDestination> func)
        {
            _func = func;
        }

        public TDestination Project(TSource source) => _func(source);
    }
}
