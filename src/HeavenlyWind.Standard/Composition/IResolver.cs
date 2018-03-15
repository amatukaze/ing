using System;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IResolver
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
    }
}
