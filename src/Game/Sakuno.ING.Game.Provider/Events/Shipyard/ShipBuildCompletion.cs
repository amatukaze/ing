using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public readonly struct ShipBuildCompletion
    {
        public ShipBuildCompletion(RawShip ship, IReadOnlyCollection<RawEquipment> equipments)
        {
            Ship = ship;
            Equipments = equipments;
        }

        public RawShip Ship { get; }
        public IReadOnlyCollection<RawEquipment> Equipments { get; }
    }
}
