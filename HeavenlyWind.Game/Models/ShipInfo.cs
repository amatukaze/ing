using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    using AbyssalShipClassEnum = AbyssalShipClass;

    public class ShipInfo : RawDataWrapper<RawShipInfo>, IID
    {
        public static ShipInfo Dummy { get; } = new ShipInfo(new RawShipInfo() { ID = -1, Name = "?" });

        public int ID => RawData.ID;

        public int SortNumber => RawData.SortNumber;

        public string Name => RawData.Name;
        public string NameReading => RawData.NameReading;

        string r_NameWithoutLateModel;

        public ShipType Type
        {
            get
            {
                ShipType rResult;
                if (KanColleGame.Current.MasterInfo.ShipTypes.TryGetValue(RawData.Type, out rResult))
                    return rResult;
                else
                    return ShipType.Dummy;
            }
        }

        public int Rarity => RawData.Rarity;

        #region Paramater

        public int HPMinimum => RawData.HP[0];
        public int HPMaximum => RawData.HP[1];
        public int ArmorMinimum => RawData.Armor[0];
        public int ArmorMaximum => RawData.Armor[1];
        public int FirepowerMinimum => RawData.Firepower[0];
        public int FirepowerMaximum => RawData.Firepower[1];
        public int AAMinimum => RawData.AA[0];
        public int AAMaximum => RawData.AA[1];
        public int TorpedoMinimum => RawData.Torpedo[0];
        public int TorpedoMaximum => RawData.Torpedo[1];
        public int LuckMinimum => RawData.Luck[0];
        public int LuckMaximum => RawData.Luck[1];

        public ShipSpeed Speed => RawData.Speed;

        public int Range => RawData.Range;

        #endregion

        public int MaxFuelConsumption => RawData.MaxFuelConsumption;
        public int MaxBulletConsumption => RawData.MaxBulletConsumption;

        public int SlotCount => RawData.SlotCount;
        public int[] PlaneCountInSlot => RawData.PlaneCountInSlot;

        public int? RemodelingMinimumLevel => RawData.RemodelingMinimumLevel == 0 ? (int?)null : RawData.RemodelingMinimumLevel;
        public ShipInfo ShipAfterRemodeling => RawData.ShipIDAfterRemodel == 0 ? null : KanColleGame.Current.MasterInfo.Ships[RawData.ShipIDAfterRemodel];

        public bool IsLandBase => Speed == ShipSpeed.None;

        public bool IsAbyssalShip => ID > 500;
        public AbyssalShipClass? AbyssalShipClass { get; }

        public string NameWithClass => !IsAbyssalShip ? Name : Name + NameReading;
        public string NameWithoutAbyssalShipClass => !IsAbyssalShip ? Name : r_NameWithoutLateModel;

        internal ShipInfo(RawShipInfo rpRawData) : base(rpRawData)
        {
            if (IsAbyssalShip)
            {
                if (NameReading == "elite")
                    AbyssalShipClass = AbyssalShipClassEnum.Elite;
                else if (NameReading == "flagship")
                    AbyssalShipClass = AbyssalShipClassEnum.Flagship;
                else if (Name.Contains("後期型"))
                    AbyssalShipClass = AbyssalShipClassEnum.LateModel;
                else
                    AbyssalShipClass = AbyssalShipClassEnum.Normal;

                r_NameWithoutLateModel = Name.Replace("後期型", string.Empty);
            }

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            var rTranslatedName = StringResources.Instance.Extra?.GetShipName(ID);
            if (rTranslatedName != null)
                RawData.Name = rTranslatedName;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{NameWithClass}\", ShipType = \"{Type.Name}\"";
    }
}
