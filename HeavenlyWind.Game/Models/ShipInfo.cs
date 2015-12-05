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
        public int[] PlaneCountInSlot => RawData.PlaneCountInSlot ?? Enumerable.Repeat(0, SlotCount).ToArray();

        public int? RemodelingMinimumLevel => RawData.RemodelingMinimumLevel == 0 ? (int?)null : RawData.RemodelingMinimumLevel;
        public ShipInfo ShipAfterRemodeling => RawData.ShipIDAfterRemodel == 0 ? null : KanColleGame.Current.MasterInfo.Ships[RawData.ShipIDAfterRemodel];

        public bool IsLandBase => Speed == ShipSpeed.None;

        public bool IsAbyssalShip => ID > 500 && ID <= 900;
        public AbyssalShipClass? AbyssalShipClass
        {
            get
            {
                if (ID > 500 && ID <= 900)
                {
                    if (Name.Contains("後期型") && NameReading.IsNullOrEmpty())
                        return AbyssalShipClassEnum.LateModel;
                    if (NameReading == "elite")
                        return AbyssalShipClassEnum.Elite;
                    if (NameReading == "flagship")
                        return AbyssalShipClassEnum.Flagship;

                    return AbyssalShipClassEnum.Normal;
                }
                return null;
            }
        }

        public string NameWithClass
        {
            get
            {
                if (!IsAbyssalShip || NameReading.IsNullOrEmpty() || NameReading == "-")
                    return Name;
                else
                    return $"{Name} {NameReading}";
            }
        }

        internal ShipInfo(RawShipInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{NameWithClass}\", ShipType = \"{Type.Name}\"";
    }
}
