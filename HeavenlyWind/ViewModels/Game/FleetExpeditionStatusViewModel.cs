using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetExpeditionStatusViewModel : ModelBase
    {
        FleetExpeditionStatus r_Source;

        public ExpeditionInfo Expedition => r_Source.Expedition;

        public string TimeToComplete => r_Source.TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTime => r_Source.RemainingTime.HasValue ? ((int)r_Source.RemainingTime.Value.TotalHours).ToString("D2") + r_Source.RemainingTime.Value.ToString(@"\:mm\:ss") : "--:--:--";

        internal FleetExpeditionStatusViewModel(FleetExpeditionStatus rpExpeditionStatus)
        {
            r_Source = rpExpeditionStatus;

            Observable.FromEventPattern<PropertyChangedEventArgs>(r_Source, nameof(r_Source.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName).Subscribe(OnPropertyChanged);
        }
    }
}
