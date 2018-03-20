using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    internal class Module : IModule
    {
        public void Initialize(IResolver resolver)
        {
            Context.DataService = resolver.Resolve<IDataService>();
        }
    }
}
