using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Settings;

namespace Sakuno.KanColle.Amatsukaze.Data
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
