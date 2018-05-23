using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipBuildCompletion
    {
        public ShipBuildCompletion(IRawShip ship, IReadOnlyCollection<IRawEquipment> equipments)
        {
            Ship = ship;
            Equipments = equipments;
        }

        public IRawShip Ship { get; }
        public IReadOnlyCollection<IRawEquipment> Equipments { get; }
    }
}
