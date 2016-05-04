using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentOverviewViewModel : WindowViewModel
    {
        Dictionary<EquipmentIconType, EquipmentTypeViewModel> r_TypeMap;
        public IList<EquipmentTypeViewModel> Types { get; }

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

        Dictionary<EquipmentInfo, EquipmentGroupByMasterID> r_EquipmentMap;
        public IReadOnlyList<EquipmentGroupByMasterID> Equipment { get; private set; }

        internal EquipmentOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.Equipment.Values.Select(r => r.Icon).Distinct().ToDictionary(IdentityFunction<EquipmentIconType>.Instance, r => new EquipmentTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
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
            var rGame = KanColleGame.Current;
            var rShips = rGame.Port.Ships.Values;

            r_EquipmentMap = rGame.Port.Equipment.Values.GroupBy(r => r.Info).OrderBy(r => r.Key.Type).ThenBy(r => r.Key.ID)
                .ToDictionary(r => r.Key, r => new EquipmentGroupByMasterID(r.Key, r_TypeMap[r.Key.Icon], r));

            foreach (var rShip in rShips)
                foreach (var rEquipment in rShip.EquipedEquipment)
                    r_EquipmentMap[rEquipment.Info].Update(rShip, new EquipmentGroupingKey(rEquipment.Level, rEquipment.Proficiency));

            UpdateFilterResult();
        }

        void UpdateFilterResult()
        {
            Equipment = r_EquipmentMap.Values.Where(r => r.Type.IsSelected).ToArray().AsReadOnly();
            OnPropertyChanged(nameof(Equipment));
        }
    }
}
