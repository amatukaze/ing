using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public interface IRawUseItemCount : IIdentifiable<UseItemId>
    {
        int Count { get; }
    }
}
