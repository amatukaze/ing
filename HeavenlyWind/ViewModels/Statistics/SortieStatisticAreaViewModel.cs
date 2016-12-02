using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticAreaViewModel : ModelBase
    {
        public MapAreaInfo Area { get; }

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

        public SortieStatisticAreaViewModel(MapAreaInfo rpArea)
        {
            Area = rpArea;
        }
    }
}
