using Sakuno.ING.Composition;

namespace Sakuno.ING.Data.Desktop
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DataService, IDataService>();
        }
    }
}
