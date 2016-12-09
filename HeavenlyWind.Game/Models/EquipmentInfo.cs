using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EquipmentInfo : RawDataWrapper<RawEquipmentInfo>, IID, ITranslatedName
    {
        public static EquipmentInfo Dummy { get; } = new EquipmentInfo(new RawEquipmentInfo() { ID = -1 });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetEquipmentName(ID) ?? Name;

        public int Rarity => RawData.Rarity;

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

        public int AntiBomber => Type == EquipmentType.InterceptorFighter ? Accuracy : 0;
        public int Interception => Type == EquipmentType.InterceptorFighter ? Evasion : 0;

        #endregion

        #region Type

        public EquipmentType Type => RawData.Type != null ? (EquipmentType)RawData.Type[2] : 0;
        public EquipmentIconType Icon => RawData.Type != null ? (EquipmentIconType)RawData.Type[3] : 0;

        public bool IsPlane
        {
            get
            {
                switch (Icon)
                {
                    case EquipmentIconType.CarrierBasedFighter:
                    case EquipmentIconType.CarrierBasedDiveBomber:
                    case EquipmentIconType.CarrierBasedTorpedoBomber:
                    case EquipmentIconType.CarrierBasedRecon:
                    case EquipmentIconType.Seaplane:
                    case EquipmentIconType.Autogyro:
                    case EquipmentIconType.ASAircraft:
                    case EquipmentIconType.FlyingBoat:
                    case EquipmentIconType.JetFighterBomberKikkaKai:
                    case EquipmentIconType.JetFighterBomberKeiunKai:
                        return true;

                    default: return false;
                }
            }
        }
        public bool CanParticipateInFighterCombat
        {
            get
            {
                switch (Type)
                {
                    case EquipmentType.CarrierBasedFighter:
                    case EquipmentType.CarrierBasedDiveBomber:
                    case EquipmentType.CarrierBasedTorpedoBomber:
                    case EquipmentType.SeaplaneBomber:
                    case EquipmentType.SeaplaneFighter:
                    case EquipmentType.LandBasedAttackAircraft:
                    case EquipmentType.InterceptorFighter:
                    case EquipmentType.JetPoweredFighter:
                    case EquipmentType.JetPoweredFighterBomber:
                    case EquipmentType.JetPoweredAttackAircraft:
                    case EquipmentType.JetPoweredRecon:
                        return true;

                    default: return false;
                }
            }
        }

        #endregion;

        public int DeploymentBauxiteConsumption => RawData.DeploymentBauxiteConsumption;

        public int CombatRadius => RawData.CombatRadius;

        internal EquipmentInfo(RawEquipmentInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\", Type = [{Type}, {Icon}]";
    }
}
