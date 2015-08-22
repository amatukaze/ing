using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.ViewModels.Contents;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {
        ModelBase r_Content;
        public ModelBase Content
        {
            get { return r_Content; }
            internal set
            {
                if (r_Content != value)
                {
                    r_Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public MainWindowViewModel()
        {
            Title = "Heavenly Wind";

            KanColleGame.Current.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(KanColleGame.Current.IsStarted))
                    Content = new GameInformationViewModel();
            };
        }
    }
}
