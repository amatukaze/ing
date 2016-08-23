using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class GameInformationViewModel : ModelBase
    {
        public MainWindowViewModel Owner { get; }

        public OverviewViewModel Overview { get; }
        public FleetsViewModel Fleets { get; }
        public SortieViewModel Sortie { get; }
        public QuestsViewModel Quests { get; }
        public ToolsViewModel Tools { get; }

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

        public Func<object, bool> OrphanedItemFilter { get; } = r => r is OverviewViewModel || r is SortieViewModel || r is QuestsViewModel;

        public bool IsBrowserAvailable { get; private set; }

        internal GameInformationViewModel(MainWindowViewModel rpOwner)
        {
            Owner = rpOwner;

            Fleets = new FleetsViewModel(this);

            TabItems = new ObservableCollection<object>()
            {
                (Overview = new OverviewViewModel()),
                (Sortie = new SortieViewModel()),
                (Quests = new QuestsViewModel(this)),
                (Tools = new ToolsViewModel(this)),
            };

            SelectedItem = TabItems.FirstOrDefault();

            IsBrowserAvailable = !BrowserService.Instance.NoInstalledLayoutEngines;
            PropertyChangedEventListener.FromSource(BrowserService.Instance).Add(nameof(BrowserService.Instance.NoInstalledLayoutEngines), delegate
            {
                IsBrowserAvailable = !BrowserService.Instance.NoInstalledLayoutEngines;
                OnPropertyChanged(nameof(IsBrowserAvailable));
            });
        }

        public void AddTabItem(object rpItem)
        {
            if (!TabItems.Contains(rpItem))
                TabItems.Add(rpItem);

            SelectedItem = rpItem;
        }
    }
}
