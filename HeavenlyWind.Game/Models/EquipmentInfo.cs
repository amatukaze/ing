using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EquipmentInfo : RawDataWrapper<RawEquipmentInfo>, IID
    {
        public static EquipmentInfo Dummy { get; } = new EquipmentInfo(new RawEquipmentInfo() { ID = -1 });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;

        #region Paramater

        public int Firepower => RawData.Firepower;
        public int Armor => RawData.Armor;
        public int Torpedo => RawData.Torpedo;
        public int AA => RawData.AA;
        public int DiveBomberAttack => RawData.DiveBomberAttack;
        public int ASW => RawData.ASW;
        public int Accuracy => RawData.Accuracy;
        public int Evasion => RawData.Evasion;
        public int LoS => RawData.LoS;

        #endregion

        #region Type

        public EquipmentType Type => RawData.Type != null ? (EquipmentType)RawData.Type[2] : 0;
        public EquipmentIcon Icon => RawData.Type != null ? (EquipmentIcon)RawData.Type[3] : 0;

        public bool CanParticipateInFighterCombat => 
            Type == EquipmentType.CarrierBasedFighter ||
            Type == EquipmentType.CarrierBasedDiveBomber ||
            Type == EquipmentType.CarrierBasedTorpedoBomber ||
            Type == EquipmentType.SeaplaneBomber;

        #endregion;

        internal EquipmentInfo(RawEquipmentInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\", Type = [{Type}, {Icon}]";
    }
}
