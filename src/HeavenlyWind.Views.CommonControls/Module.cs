using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            ResolveExtension.Resolver = resolver;
        }
    }
}
