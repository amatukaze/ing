using Sakuno.ING.Composition;
using Sakuno.ING.Data;

namespace Sakuno.ING.UWP.Data
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
