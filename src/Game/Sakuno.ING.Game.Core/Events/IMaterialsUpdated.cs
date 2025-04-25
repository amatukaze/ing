using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IMaterialsUpdated
{
    void Apply(ref Materials materials);
}
