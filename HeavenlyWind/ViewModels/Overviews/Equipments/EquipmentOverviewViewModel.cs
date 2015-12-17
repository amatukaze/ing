using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
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

        Dictionary<EquipmentInfo, EquipmentsGroupByMasterID> r_EquipmentMap;
        public IReadOnlyList<EquipmentsGroupByMasterID> Equipments { get; private set; }

        internal EquipmentOverviewViewModel()
        {
            Title = StringResources.Instance.Main.Window_EquipmentOverview;

            r_TypeMap = ((EquipmentIconType[])Enum.GetValues(typeof(EquipmentIconType))).Skip(1).ToDictionary(r => r, r => new EquipmentTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
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
            var rEquipments = rGame.Port.Equipments.Values;

            r_EquipmentMap = rEquipments.GroupBy(r => r.Info).OrderBy(r => r.Key.Type).ThenBy(r => r.Key.ID)
                .ToDictionary(r => r.Key, r => new EquipmentsGroupByMasterID(r.Key, r_TypeMap[r.Key.Icon], r));

            foreach (var rShip in rShips)
                foreach (var rEquipment in rShip.Equipments)
                    r_EquipmentMap[rEquipment.Info].Update(rShip, new EquipmentGroupingKey(rEquipment.Level, rEquipment.Proficiency));

            UpdateFilterResult();
        }

        void UpdateFilterResult()
        {
            Equipments = r_EquipmentMap.Values.Where(r => r.Type.IsSelected).ToArray().AsReadOnly();
            OnPropertyChanged(nameof(Equipments));
        }
    }
}
