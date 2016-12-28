using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class RepairDockViewModel : ModelBase
    {
        RepairDock r_Source;

        public int ID => r_Source.ID;
        public RepairDockState State => r_Source.State;
        public ShipInfo Ship => r_Source.Ship?.Info;

        public string TimeToComplete => r_Source.TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTime => r_Source.RemainingTime.HasValue ? ((int)r_Source.RemainingTime.Value.TotalHours).ToString("D2") + r_Source.RemainingTime.Value.ToString(@"\:mm\:ss") : "--:--:--";

        public RepairDockViewModel(RepairDock rpRepairDock)
        {
            r_Source = rpRepairDock;

            Observable.FromEventPattern<PropertyChangedEventArgs>(r_Source, nameof(r_Source.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName).Subscribe(OnPropertyChanged);
        }
    }
}
