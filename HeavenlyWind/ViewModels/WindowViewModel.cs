namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class WindowViewModel : ModelBase
    {
        string r_Title;
        public string Title
        {
            get { return r_Title; }
            set
            {
                if (r_Title != value)
                {
                    r_Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
    }
}
