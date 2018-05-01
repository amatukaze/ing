using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class ShipBuildCompletionJson : IShipBuildCompletion
    {
        public BuildingDockJson[] api_kdock;
        public ShipJson api_ship;
        IRawShip IShipBuildCompletion.Ship => api_ship;
        public EquipmentJson[] api_slotitem;
        IReadOnlyCollection<IRawEquipment> IShipBuildCompletion.Equipments => api_slotitem ?? Array.Empty<IRawEquipment>();
    }
}
