using Sakuno.KanColle.Amatsukaze.ViewModels.Game;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class GameInformationViewModel : ModelBase
    {
        public OverviewViewModel Overview { get; }
        public FleetsViewModel Fleets { get; }
        public SortieViewModel Sortie { get; }
        public QuestsViewModel Quests { get; }

        public IList<object> TabItems { get; }

        object r_SelectedItem;
        public object SelectedItem
        {
            get { return r_SelectedItem; }
            set
            {
                if (r_SelectedItem != value)
                {
                    r_SelectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        internal GameInformationViewModel()
        {
            Fleets = new FleetsViewModel(this);

            TabItems = new ObservableCollection<object>()
            {
                (Overview = new OverviewViewModel()),
                (Sortie = new SortieViewModel()),
                (Quests = new QuestsViewModel(this)),
            };

            SelectedItem = TabItems.FirstOrDefault();
        }

        public void AddTabItem(object rpItem)
        {
            if (!TabItems.Contains(rpItem))
                TabItems.Add(rpItem);

            SelectedItem = rpItem;
        }
    }
}
