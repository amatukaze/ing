using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipOverviewViewModel : WindowViewModel
    {
        Dictionary<ShipType, ShipTypeViewModel> r_TypeMap;
        public IReadOnlyCollection<ShipTypeViewModel> Types { get; }

        bool? r_SelectAllTypes = true;
        public bool? SelectAllTypes
        {
            get { return r_SelectAllTypes; }
            set
            {
                if (r_SelectAllTypes != value)
                {
                    r_SelectAllTypes = value;
                    if (r_SelectAllTypes.HasValue)
                    {
                        foreach (var rType in Types)
                            rType.SetIsSelectedWithoutCallback(r_SelectAllTypes.Value);

                        UpdateSelection();
                        OnPropertyChanged(nameof(SelectAllTypes));
                    }
                }
            }
        }

        ShipViewModel[] r_Ships;
        public IReadOnlyCollection<ShipViewModel> Ships { get; private set; }

        internal ShipOverviewViewModel()
        {
            Title = StringResources.Instance.Main.Window_ShipOverview;

            r_TypeMap = KanColleGame.Current.MasterInfo.ShipTypes.Values.Where(r => r.ID != 15).ToDictionary(r => r, r => new ShipTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
            Types = r_TypeMap.Values.ToArray().AsReadOnly();

            Task.Run(new Action(UpdateCore));
        }

        void UpdateSelection()
        {
            var rTypeCount = Types.Count;
            var rSelectedCount = Types.Count(r => r.IsSelected);

            if (rSelectedCount == 0)
                r_SelectAllTypes = false;
            else if (rSelectedCount == rTypeCount)
                r_SelectAllTypes = true;
            else
                r_SelectAllTypes = null;

            UpdateFilterResult();
            OnPropertyChanged(nameof(SelectAllTypes));
        }

        void UpdateCore()
        {
            r_Ships = KanColleGame.Current.Port.Ships.Values.Select((r, i) => new ShipViewModel(i, r, r_TypeMap[r.Info.Type])).ToArray();
            UpdateFilterResult();
        }

        void UpdateFilterResult()
        {
            Ships = r_Ships.Where(r => r.Type.IsSelected).ToArray().AsReadOnly();
            OnPropertyChanged(nameof(Ships));
        }
    }
}
