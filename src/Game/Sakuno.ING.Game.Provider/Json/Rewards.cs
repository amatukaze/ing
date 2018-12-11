using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal sealed class Rewards : IRawRewards
    {
        public List<UseItemRecord> UseItem { get; } = new List<UseItemRecord>();
        IReadOnlyCollection<UseItemRecord> IRawRewards.UseItem => UseItem;

        public List<EquipmentRecord> Equipment { get; } = new List<EquipmentRecord>();
        IReadOnlyCollection<EquipmentRecord> IRawRewards.Equipment => Equipment;

        public List<FurnitureId> Furniture { get; } = new List<FurnitureId>();
        IReadOnlyCollection<FurnitureId> IRawRewards.Furniture => Furniture;

        public List<ShipInfoId> Ship { get; } = new List<ShipInfoId>();
        IReadOnlyCollection<ShipInfoId> IRawRewards.Ship => Ship;
    }
}
