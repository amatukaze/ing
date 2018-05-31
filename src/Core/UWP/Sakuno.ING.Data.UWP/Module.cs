using Sakuno.ING.Composition;

namespace Sakuno.ING.Data.UWP
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
