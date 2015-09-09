using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class BuildingDockViewModel : ModelBase
    {
        BuildingDock r_Source;

        public int ID => r_Source.ID;
        public BuildingDockState State => r_Source.State;
        public string Ship => r_Source.Ship?.Name;

        public string TimeToComplete => r_Source.TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTime => r_Source.RemainingTime.HasValue ? ((int)r_Source.RemainingTime.Value.TotalHours).ToString("D2") + r_Source.RemainingTime.Value.ToString(@"\:mm\:ss") : "--:--:--";

        public bool? IsLargeShipConstruction => r_Source.IsLargeShipConstruction;

        public int FuelConsumption => r_Source.FuelConsumption;
        public int BulletConsumption => r_Source.BulletConsumption;
        public int SteelConsumption => r_Source.SteelConsumption;
        public int BauxiteConsumption => r_Source.BauxiteConsumption;
        public int DevelopmentMaterialConsumption => r_Source.DevelopmentMaterialConsumption;

        internal BuildingDockViewModel(BuildingDock rpBuildingDock)
        {
            r_Source = rpBuildingDock;

            Observable.FromEventPattern<PropertyChangedEventArgs>(r_Source, nameof(r_Source.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName).Subscribe(OnPropertyChanged);
        }
    }
}
