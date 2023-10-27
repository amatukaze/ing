using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IConstructionDockUpdated
{
    ConstructionDockId Id { get; }
}
