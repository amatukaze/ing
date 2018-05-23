using Sakuno.ING.Composition;
using Sakuno.ING.ViewModels.Layout;

namespace Sakuno.ING.ViewModels
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<LayoutSetting>();
        }
        public void Initialize(IResolver resolver) { }
    }
}
