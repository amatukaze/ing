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
        public QuestsViewModel Quests { get; }

        public IList<TabItemViewModel> TabItems { get; }

        TabItemViewModel r_SelectedItem;
        public TabItemViewModel SelectedItem
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
            TabItems = new ObservableCollection<TabItemViewModel>()
            {
                (Overview = new OverviewViewModel()),
                (Fleets = new FleetsViewModel(this)),
                (Quests = new QuestsViewModel()),
            };

            SelectedItem = TabItems.FirstOrDefault();
        }
    }
}
