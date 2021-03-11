using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
#nullable disable
    public sealed class ShipModernization
    {
        public ShipId ShipId { get; }
        public IReadOnlyCollection<ShipId> ConsumedShipIds { get; }

        public bool IsSuccess { get; }

        public RawShip NewRawData { get; }
        public bool RemoveSlotItems { get; }

        public ShipModernization(ShipId shipId, IReadOnlyCollection<ShipId> consumedShipIds, bool isSuccess, RawShip newRawData, bool removeSlotItems)
        {
            ShipId = shipId;
            ConsumedShipIds = consumedShipIds;
            IsSuccess = isSuccess;
            NewRawData = newRawData;
            RemoveSlotItems = removeSlotItems;
        }
    }
#nullable enable
}
