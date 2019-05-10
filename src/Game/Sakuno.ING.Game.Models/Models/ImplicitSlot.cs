using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class ImplicitSlot : Slot
    {
        public override Equipment Equipment { get; }

        public ImplicitSlot(EquipmentInfo equipmentInfo, ClampedValue aircraft = default, int improvementLevel = 0, int airProficiency = 0)
        {
            if (equipmentInfo != null)
                Equipment = new ImplicitEquipment(equipmentInfo, improvementLevel, airProficiency);
            Aircraft = aircraft;
            DoCalculations();
        }

        public class ImplicitEquipment : Equipment
        {
            public ImplicitEquipment(EquipmentInfo equipmentInfo, int improvementLevel = 0, int airProficiency = 0)
            {
                Info = equipmentInfo;
                ImprovementLevel = improvementLevel;
                AirProficiency = airProficiency;
            }
        }
    }
}
