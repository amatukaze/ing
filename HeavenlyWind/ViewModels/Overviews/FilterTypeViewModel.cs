using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public abstract class FilterTypeViewModel : ModelBase
    {
        string r_Name;
        public string Name
        {
            get { return r_Name; }
            internal set
            {
                if (r_Name != value)
                {
                    r_Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Action IsSelectedChangedCallback { get; internal set; }

        bool r_IsSelected;
        public bool IsSelected
        {
            get { return r_IsSelected; }
            internal set
            {
                if (r_IsSelected != value)
                {
                    r_IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    IsSelectedChangedCallback?.Invoke();
                }
            }
        }

        public void SetIsSelectedWithoutCallback(bool rpValue)
        {
            r_IsSelected = rpValue;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
