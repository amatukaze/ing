using System.Runtime.CompilerServices;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;

[assembly: InternalsVisibleTo("Sakuno.ING.Game.Provider.Test")]
namespace Sakuno.ING.Game.Models
{
    internal class Module : IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterType<NavalBase>();
        }
    }
}
