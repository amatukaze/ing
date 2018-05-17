using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class FleetConditionRegenerationViewModel : ModelBase
    {
        FleetConditionRegeneration r_Source;

        public string TimeToComplete => r_Source.TimeToComplete?.LocalDateTime.ToString();
        public string RemainingTime => r_Source.RemainingTime.HasValue ? ((int)r_Source.RemainingTime.Value.TotalMinutes).ToString("D2") + r_Source.RemainingTime.Value.ToString(@"\:ss") : "--:--:--";

        internal FleetConditionRegenerationViewModel(FleetConditionRegeneration rpSource)
        {
            r_Source = rpSource;

            Observable.FromEventPattern<PropertyChangedEventArgs>(r_Source, nameof(r_Source.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName).Subscribe(OnPropertyChanged);
        }
    }
}
