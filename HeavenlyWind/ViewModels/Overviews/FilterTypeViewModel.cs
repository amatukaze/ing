namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class FilterTypeViewModel<T> : ModelBase
    {
        public T Type { get; }

        bool r_IsSelected;
        public bool IsSelected
        {
            get { return r_IsSelected; }
            set
            {
                if (r_IsSelected != value)
                {
                    r_IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public FilterTypeViewModel(T rpType)
        {
            Type = rpType;
        }
    }
}
