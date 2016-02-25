using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class ExpeditionResultPrediction : ModelBase
    {
        ExpeditionInfo2 r_Info;
        Fleet r_Fleet;

        public int ID => r_Fleet.ID;

        public bool Success { get; private set; }

        public bool FlagshipLevel { get; private set; }
        public bool ShipCount { get; private set; }

        public bool? FlagshipType { get; private set; }

        public bool? TotalLevel { get; private set; }

        public bool? DrumCount { get; private set; }
        public bool? DrumCarrierCount { get; private set; }

        public bool? ShipRequirements { get; private set; }

        internal ExpeditionResultPrediction(ExpeditionInfo2 rpInfo, Fleet rpFleet)
        {
            r_Info = rpInfo;
            r_Fleet = rpFleet;

            Check();
        }

        public void Check()
        {
            var rShips = r_Fleet.Ships;

            FlagshipLevel = rShips[0].Level >= r_Info.FlagshipLevel;
            ShipCount = rShips.Count >= r_Info.ShipCount;

            if (r_Info.FlagshipType.HasValue)
                FlagshipType = rShips[0].Info.Type.ID == r_Info.FlagshipType.Value;

            if (r_Info.TotalLevel.HasValue)
                TotalLevel = r_Fleet.Status.TotalLevel >= r_Info.TotalLevel.Value;

            if (r_Info.DrumRequirement != null)
            {
                DrumCount = rShips.Count(r => r.Slots.Any(rpSlot => rpSlot.Equipment.Info.Icon == EquipmentIconType.DrumCanister)) >= r_Info.DrumRequirement.CarrierCount;
                DrumCarrierCount = rShips.Sum(r => r.Slots.Count(rpSlot => rpSlot.Equipment.Info.Icon == EquipmentIconType.DrumCanister)) >= r_Info.DrumRequirement.TotalCount;
            }

            if (r_Info.ShipRequirements != null)
                ShipRequirements = r_Info.ShipRequirements.All(r => r.Types.All(rpType => rShips.Count(rpShip => rpShip.Info.Type.ID == rpType) >= r.Count));

            Success = FlagshipLevel &&
                ShipCount &&
                (!FlagshipType.HasValue || FlagshipType.Value) &&
                (!TotalLevel.HasValue || TotalLevel.Value) &&
                (!DrumCount.HasValue || DrumCount.Value) &&
                (!DrumCarrierCount.HasValue || DrumCarrierCount.Value) &&
                (!ShipRequirements.HasValue || ShipRequirements.Value);

            OnPropertyChanged(nameof(FlagshipLevel));
            OnPropertyChanged(nameof(ShipCount));
            OnPropertyChanged(nameof(FlagshipType));
            OnPropertyChanged(nameof(TotalLevel));
            OnPropertyChanged(nameof(DrumCount));
            OnPropertyChanged(nameof(DrumCarrierCount));
            OnPropertyChanged(nameof(ShipRequirements));
            OnPropertyChanged(nameof(Success));
        }
    }
}
