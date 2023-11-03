using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IShipInfoUpdated : IIdentifiable<ShipInfoId>
{
    string Name { get; }
    ShipTypeId TypeId { get; }
    int ClassId { get; }

    int RemodelLevel { get; }
    ShipInfoId? RemodelTo { get; }
    Materials RemodelConsumption { get; }

    ShipSpeed Speed { get; }
    FireRange FireRange { get; }

    int SlotCount { get; }
    IReadOnlyList<int> PlaneCapacities { get; }

    TimeSpan ConstructionTime { get; }

    int Rarity { get; }

    int FuelConsumption { get; }
    int BulletConsumption { get; }
}
