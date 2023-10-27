using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IExpeditionInfoUpdated
{
    ExpeditionId Id { get; }
    string Name { get; }
}
