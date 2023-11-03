using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IShipUpdated : IIdentifiable<ShipId>
{
    ShipInfoId MasterId { get; }

    int Level { get; }

    int Fuel { get; }
    int Bullet { get; }

    int Morale { get; }

    bool IsLocked { get; }

    int? SortieLockingTagId { get; }
}
