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
        Dictionary<EquipmentInfo, EquipmentsGroupByMasterID> r_EquipmentMap;
        public IReadOnlyList<EquipmentsGroupByMasterID> Equipments { get; private set; }

        internal EquipmentOverviewViewModel()
        {
            Title = StringResources.Instance.Main.Window_EquipmentOverview;

            Task.Run(new Action(UpdateCore));
        }

        void UpdateCore()
        {
            var rGame = KanColleGame.Current;
            var rShips = rGame.Port.Ships.Values;
            var rEquipments = rGame.Port.Equipments.Values;

            r_EquipmentMap = rEquipments.GroupBy(r => r.Info).OrderBy(r => r.Key.Type).ThenBy(r => r.Key.ID)
                .ToDictionary(r => r.Key, r => new EquipmentsGroupByMasterID(r.Key, r));

            foreach (var rShip in rShips)
                foreach (var rEquipment in rShip.Equipments)
                    r_EquipmentMap[rEquipment.Info].Update(rShip, new EquipmentGroupingKey(rEquipment.Level, rEquipment.Proficiency));

        }

    }
}
