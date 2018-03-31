using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Data;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class Module : IExposableModule
    {
        private DataService service = new DataService();
        public void Expose(IBuilder builder)
        {
            builder.RegisterInstance<IDataService>(service);
        }
        public void Initialize(IResolver resolver)
        {
            service.InitializeAsync().Wait();
        }
    }
}
