using System;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipStatus : ModelBase
    {
        Ship r_Ship;

        public bool IsCompleted => FirepowerBase.IsMax && TorpedoBase.IsMax && AABase.IsMax && ArmorBase.IsMax;

        public int Firepower => r_Ship.RawData.Firepower[0];
        ShipModernizationStatus r_FirepowerBase;
        public ShipModernizationStatus FirepowerBase
        {
            get { return r_FirepowerBase; }
            private set
            {
                r_FirepowerBase = value;
                OnPropertyChanged(nameof(FirepowerBase));
            }
        }

        public int Torpedo => r_Ship.RawData.Torpedo[0];
        ShipModernizationStatus r_TorpedoBase;
        public ShipModernizationStatus TorpedoBase
        {
            get { return r_TorpedoBase; }
            private set
            {
                r_TorpedoBase = value;
                OnPropertyChanged(nameof(TorpedoBase));
            }
        }

        public int AA => r_Ship.RawData.AA[0];
        ShipModernizationStatus r_AABase;
        public ShipModernizationStatus AABase
        {
            get { return r_AABase; }
            private set
            {
                r_AABase = value;
                OnPropertyChanged(nameof(AABase));
            }
        }

        public int Armor => r_Ship.RawData.Armor[0];
        ShipModernizationStatus r_ArmorBase;
        public ShipModernizationStatus ArmorBase
        {
            get { return r_ArmorBase; }
            private set
            {
                r_ArmorBase = value;
                OnPropertyChanged(nameof(ArmorBase));
            }
        }

        public int Evasion => r_Ship.RawData.Evasion[0];

        public int ASW => r_Ship.RawData.ASW[0];

        public int LoS => r_Ship.RawData.LoS[0];

        ShipModernizationStatus r_Luck;
        public ShipModernizationStatus Luck
        {
            get { return r_Luck; }
            private set
            {
                r_Luck = value;
                OnPropertyChanged(nameof(Luck));
            }
        }

        internal ShipStatus(Ship rpShip)
        {
            r_Ship = rpShip;
        }

        internal void Update(ShipInfo rpInfo, RawShip rpData)
        {
            FirepowerBase = new ShipModernizationStatus(rpInfo.FirepowerMinimum, rpInfo.FirepowerMaximum, rpData.ModernizedStatus[0]);
            TorpedoBase = new ShipModernizationStatus(rpInfo.TorpedoMinimum, rpInfo.TorpedoMaximum, rpData.ModernizedStatus[1]);
            AABase = new ShipModernizationStatus(rpInfo.AAMinimum, rpInfo.AAMaximum, rpData.ModernizedStatus[2]);
            ArmorBase = new ShipModernizationStatus(rpInfo.ArmorMinimum, rpInfo.ArmorMaximum, rpData.ModernizedStatus[3]);
            Luck = new ShipModernizationStatus(rpInfo.LuckMinimum, rpInfo.LuckMaximum, rpData.ModernizedStatus[4]);
        }
    }
}
