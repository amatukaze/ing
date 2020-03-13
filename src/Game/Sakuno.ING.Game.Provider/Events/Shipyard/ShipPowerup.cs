﻿using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public sealed class ShipPowerup
    {
        public ShipPowerup(ShipId shipId, IReadOnlyCollection<ShipId> consumedShipIds, bool isSuccess, RawShip updatedTo, bool dismantleEquipments)
        {
            ShipId = shipId;
            ConsumedShipIds = consumedShipIds;
            IsSuccess = isSuccess;
            UpdatedTo = updatedTo;
            DismantleEquipments = dismantleEquipments;
        }

        public ShipId ShipId { get; }
        public IReadOnlyCollection<ShipId> ConsumedShipIds { get; }
        public bool IsSuccess { get; }
        public RawShip UpdatedTo { get; }
        public bool DismantleEquipments { get; }
    }
}
