using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public interface IShipBuildCompletion
    {
        IRawShip Ship { get; }
        IReadOnlyCollection<IRawEquipment> Equipments { get; }
    }
}
