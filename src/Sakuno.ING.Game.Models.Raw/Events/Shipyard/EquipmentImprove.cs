using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class EquipmentImprove
    {
        public EquipmentImprove(int equipmentId, int recipeId, bool guaranteedSuccess, bool isSuccess, IRawEquipment updatedTo, IReadOnlyCollection<int> consumedEquipmentIds)
        {
            EquipmentId = equipmentId;
            RecipeId = recipeId;
            GuaranteedSuccess = guaranteedSuccess;
            IsSuccess = isSuccess;
            UpdatedTo = updatedTo;
            ConsumedEquipmentIds = consumedEquipmentIds;
        }

        public int EquipmentId { get; }
        public int RecipeId { get; }
        public bool GuaranteedSuccess { get; }
        public bool IsSuccess { get; }
        public IRawEquipment UpdatedTo { get; }
        public IReadOnlyCollection<int> ConsumedEquipmentIds { get; }
    }
}
