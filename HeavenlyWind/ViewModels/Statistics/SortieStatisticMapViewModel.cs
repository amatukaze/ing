using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Windows.Input;
using EventMapDifficultyEnum = Sakuno.KanColle.Amatsukaze.Game.Models.EventMapDifficulty;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticMapViewModel : ModelBase
    {
        public IMapMasterInfo Map { get; }
        public bool IsEventMap { get; }
        public EventMapDifficulty? EventMapDifficulty { get; }

        bool r_IsSelected = true;
        public bool IsSelected
        {
            get { return r_IsSelected; }
            set
            {
                if (r_IsSelected != value)
                {
                    r_IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    IsSelectedChangedCallback?.Invoke();
                }
            }
        }

        public Action IsSelectedChangedCallback { get; internal set; }

        public ICommand SelectThisMapOnlyCommand { get; internal set; }

        public SortieStatisticMapViewModel(IMapMasterInfo rpMap, EventMapDifficulty rpDifficulty)
        {
            Map = rpMap;
            IsEventMap = rpDifficulty != EventMapDifficultyEnum.None;
            EventMapDifficulty = rpDifficulty;
        }

        public void SetIsSelectedWithoutCallback(bool rpValue)
        {
            r_IsSelected = rpValue;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
