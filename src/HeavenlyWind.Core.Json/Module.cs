using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    [ModuleTargetStandardVersion(StandardInformation.TargetVersion)]
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<JsonService, IJsonService>();
        }

        public void Initialize(IResolver resolver)
        {

        }
    }
}
