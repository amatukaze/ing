using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public interface IMaterialsUpdate
    {
        void Apply(ref Materials materials);
    }
}
