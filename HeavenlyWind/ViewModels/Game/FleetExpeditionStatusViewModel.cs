using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetExpeditionStatusViewModel : ModelBase
    {
        Fleet r_Fleet;
        FleetExpeditionStatus r_Source;

        public ExpeditionInfo Expedition => r_Source.Expedition;

        public string TimeToComplete => r_Source.TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTime => r_Source.RemainingTime.HasValue ? ((int)r_Source.RemainingTime.Value.TotalHours).ToString("D2") + r_Source.RemainingTime.Value.ToString(@"\:mm\:ss") : "--:--:--";

        public ExpeditionYield Yield { get; private set; }

        internal FleetExpeditionStatusViewModel(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
            r_Source = rpFleet.ExpeditionStatus;

            PropertyChangedEventListener.FromSource(r_Source).Add(nameof(r_Source.Expedition), (s, e) => UpdateExpeditionYield());
            UpdateExpeditionYield();

            Observable.FromEventPattern<PropertyChangedEventArgs>(r_Source, nameof(r_Source.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName).Subscribe(OnPropertyChanged);
        }

        void UpdateExpeditionYield()
        {
            if (r_Source.Expedition == null)
                Yield = null;
            else
            {
                var rInfo = ExpeditionService.Instance.GetInfo(r_Source.Expedition.ID);
                if (rInfo != null)
                    Yield = new ExpeditionYield(r_Fleet, rInfo);
                else
                    Yield = null;
            }
            OnPropertyChanged(nameof(Yield));
        }

        public class ExpeditionYield
        {
            public int Fuel { get; }
            public int Bullet { get; }
            public int Steel { get; }
            public int Bauxite { get; }

            public int FuelGS { get; }
            public int BulletGS { get; }
            public int SteelGS { get; }
            public int BauxiteGS { get; }

            public ExpeditionYield(Fleet rpFleet, ExpeditionInfo2 rpInfo)
            {
                var rExpedition = rpFleet.ExpeditionStatus.Expedition;

                var rLandingCraftCount = rpFleet.Ships.SelectMany(r => r.Slots).Select(r => r.Equipment.Info.Icon).Count(r => r == EquipmentIconType.LandingCraft);
                var rRate = 1.0 + rLandingCraftCount * .05;

                var rFuel = rpInfo.Resources.Fuel * rRate;
                var rBullet = rpInfo.Resources.Bullet * rRate;
                var rSteel = rpInfo.Resources.Steel * rRate;
                var rBauxite = rpInfo.Resources.Bauxite * rRate;

                var rFuelConsumption = (int)rpFleet.Ships.Sum(r => r.Info.MaxFuelConsumption * rExpedition.FuelConsumption * (r.IsMarried ? .85 : 1.0));
                var rBulletConsumption = (int)rpFleet.Ships.Sum(r => r.Info.MaxBulletConsumption * rExpedition.BulletConsumption * (r.IsMarried ? .85 : 1.0));

                Fuel = (int)rFuel - rFuelConsumption;
                Bullet = (int)rBullet - rBulletConsumption;
                FuelGS = (int)(rFuel * 1.5) - rFuelConsumption;
                BulletGS = (int)(rBullet * 1.5) - rBulletConsumption;

                Steel = (int)rSteel;
                Bauxite = (int)rBauxite;
                SteelGS = (int)(Steel * 1.5);
                BauxiteGS = (int)(rBauxite * 1.5);
            }
        }
    }
}
