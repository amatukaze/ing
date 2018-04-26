using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<SettingsManager, ISettingsManager>();
        }

        public void Initialize(IResolver resolver)
        {
            Context.DataService = resolver.Resolve<IDataService>();
        }
    }
}
