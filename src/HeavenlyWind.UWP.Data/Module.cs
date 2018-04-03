using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Data;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DataService, IDataService>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
