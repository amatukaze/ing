using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class FleetExpeditionStatusViewModel : ModelBase
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
            if (r_Source.Expedition == null || !r_Source.Expedition.CanReturn)
                Yield = null;
            else
            {
                ExpeditionService.Instance.WaitForInitialization();

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

            public double HourlyFuel { get; }
            public double HourlyBullet { get; }
            public double HourlySteel { get; }
            public double HourlyBauxite { get; }

            public double HourlyFuelGS { get; }
            public double HourlyBulletGS { get; }
            public double HourlySteelGS { get; }
            public double HourlyBauxiteGS { get; }

            public ExpeditionYield(Fleet rpFleet, ExpeditionInfo2 rpInfo)
            {
                var rExpedition = rpFleet.ExpeditionStatus.Expedition;

                var rFuelConsumption = 0;
                var rBulletConsumption = 0;

                var rLandingCraftCount = 0;
                var rLandingCraftLevel = 0;
                var rLandingCraftBonusRate = 1.0;

                var rTDLCCount = 0;
                var rDLCCount = 0;

                foreach (var rShip in rpFleet.Ships)
                {
                    var rFactor = rShip.IsMarried ? .85 : 1.0;

                    rFuelConsumption += (int)(rShip.Info.MaxFuelConsumption * rExpedition.FuelConsumption * rFactor);
                    rBulletConsumption += (int)(rShip.Info.MaxBulletConsumption * rExpedition.BulletConsumption * rFactor);

                    if (rShip.Info.ID == 487)
                        rLandingCraftBonusRate += .05;

                    foreach (var rSlot in rShip.Slots)
                    {
                        switch (rSlot.Equipment.Info.ID)
                        {
                            case 68:
                                rLandingCraftBonusRate += .05;
                                rDLCCount++;
                                break;

                            case 166:
                                rLandingCraftBonusRate += .02;
                                break;

                            case 167:
                                rLandingCraftBonusRate += .01;
                                break;

                            case 193:
                                rLandingCraftBonusRate += .05;
                                rTDLCCount++;
                                break;

                            default:
                                continue;
                        }

                        rLandingCraftCount++;
                        rLandingCraftLevel += rSlot.Equipment.Level;
                    }
                }

                rLandingCraftBonusRate = Math.Min(rLandingCraftBonusRate, 1.2);

                if (rLandingCraftCount > 0)
                    rLandingCraftBonusRate += rLandingCraftBonusRate * rLandingCraftLevel * .01 / rLandingCraftCount;

                switch (rTDLCCount)
                {
                    case 0:
                    case 1:
                    case 2:
                        rLandingCraftBonusRate += rTDLCCount * .02;
                        break;

                    case 3:
                        switch (rDLCCount)
                        {
                            case 0:
                            case 1:
                                rLandingCraftBonusRate += .05;
                                break;

                            case 2:
                                rLandingCraftBonusRate += .052;
                                break;

                            default:
                                rLandingCraftBonusRate += .054;
                                break;
                        }
                        break;

                    default:
                        switch (rDLCCount)
                        {
                            case 0:
                                rLandingCraftBonusRate += .054;
                                break;

                            case 1:
                                rLandingCraftBonusRate += .056;
                                break;

                            case 2:
                                rLandingCraftBonusRate += .058;
                                break;

                            case 3:
                                rLandingCraftBonusRate += .059;
                                break;

                            default:
                                rLandingCraftBonusRate += .06;
                                break;
                        }
                        break;
                }

                var rFuel = rpInfo.RewardResources.Fuel * rLandingCraftBonusRate;
                var rBullet = rpInfo.RewardResources.Bullet * rLandingCraftBonusRate;
                var rSteel = rpInfo.RewardResources.Steel * rLandingCraftBonusRate;
                var rBauxite = rpInfo.RewardResources.Bauxite * rLandingCraftBonusRate;

                Fuel = (int)rFuel - rFuelConsumption;
                Bullet = (int)rBullet - rBulletConsumption;
                FuelGS = (int)(rFuel * 1.5) - rFuelConsumption;
                BulletGS = (int)(rBullet * 1.5) - rBulletConsumption;

                Steel = (int)rSteel;
                Bauxite = (int)rBauxite;
                SteelGS = (int)(Steel * 1.5);
                BauxiteGS = (int)(rBauxite * 1.5);

                var rTotalHours = TimeSpan.FromMinutes(rExpedition.Time).TotalHours;
                var rHourlyFuelConsumption = rFuelConsumption / rTotalHours;
                var rHourlyBulletConsumption = rBulletConsumption / rTotalHours;

                HourlyFuel = rFuel / rTotalHours - rHourlyFuelConsumption;
                HourlyBullet = rBullet / rTotalHours - rHourlyBulletConsumption;
                HourlyFuelGS = rFuel * 1.5 / rTotalHours - rHourlyFuelConsumption;
                HourlyBulletGS = rBullet * 1.5 / rTotalHours - rHourlyBulletConsumption;

                HourlySteel = rSteel / rTotalHours;
                HourlyBauxite = rBauxite / rTotalHours;
                HourlySteelGS = Steel * 1.5 / rTotalHours;
                HourlyBauxiteGS = rBauxite * 1.5 / rTotalHours;
            }
        }
    }
}
