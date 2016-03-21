using Sakuno.KanColle.Amatsukaze.Controls;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipOverviewViewModel : WindowViewModel
    {
        Dictionary<ShipTypeInfo, ShipTypeViewModel> r_TypeMap;
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

        Dictionary<int, ShipViewModel> r_ShipVMs = new Dictionary<int, ShipViewModel>(100);
        ShipViewModel[] r_Ships;
        public IReadOnlyCollection<ShipViewModel> Ships { get; private set; }

        string r_SortingColumn;

        public int ShipLockingColumnWidth => ShipLockingService.Instance?.ShipLocking?.Count > 0 && KanColleGame.Current.MasterInfo.EventMapCount > 0 ? 30 : 0;

        internal ShipOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.ShipTypes.Values.Where(r => r.ID != 15).ToDictionary(r => r, r => new ShipTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
            Types = r_TypeMap.Values.ToArray().AsReadOnly();

            Task.Factory.StartNew(UpdateCore);
        }

        internal void Sort(GridViewColumnHeader rpColumnHeader)
        {
            var rColumn = Data.GetContent(rpColumnHeader.Column);
            if (rColumn == null)
                return;

            r_SortingColumn = rColumn;
            Task.Factory.StartNew(UpdateCore);
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

            Task.Factory.StartNew(UpdateCore);
            OnPropertyChanged(nameof(SelectAllTypes));
        }

        void UpdateCore()
        {
            r_Ships = KanColleGame.Current.Port.Ships.Values.Select(r =>
            {
                ShipViewModel rResult;
                if (!r_ShipVMs.TryGetValue(r.ID, out rResult))
                    rResult = new ShipViewModel(r, r_TypeMap[r.Info.Type]);

                return rResult;
            }).ToArray();

            var rShips = r_Ships.Where(r => r.Type.IsSelected);
            switch (r_SortingColumn)
            {
                case "ID":
                    rShips = rShips.OrderBy(r => r.Ship.ID);
                    break;

                case "Level":
                    rShips = rShips.OrderByDescending(r => r.Ship.Level).ThenBy(r => r.Ship.ExperienceToNextLevel);
                    break;

                case "Condition":
                    rShips = rShips.OrderByDescending(r => r.Ship.Condition);
                    break;

                case "Firepower":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.FirepowerBase.Current);
                    break;
                case "Torpedo":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.TorpedoBase.Current);
                    break;
                case "AA":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.AABase.Current);
                    break;
                case "Armor":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.ArmorBase.Current);
                    break;
                case "Luck":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.LuckBase.Current);
                    break;

                case "Evasion":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.Evasion);
                    break;
                case "ASW":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.ASW);
                    break;
                case "LoS":
                    rShips = rShips.OrderByDescending(r => r.Ship.Status.LoS);
                    break;
            }

            Ships = rShips.ToArray().AsReadOnly();
            OnPropertyChanged(nameof(Ships));
        }
    }
}
