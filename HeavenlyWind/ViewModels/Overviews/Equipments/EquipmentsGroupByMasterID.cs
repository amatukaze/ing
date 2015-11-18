using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews.Equipments
{
    public class EquipmentsGroupByMasterID
    {
        public EquipmentInfo Info { get; }

        Dictionary<EquipmentGroupingKey, EquipmentsGroupByLevel> r_LevelMap;
        public IReadOnlyCollection<EquipmentsGroupByLevel> Levels { get; }

        public int Count => r_LevelMap.Values.Sum(r => r.Count);
        public int RemainingCount => r_LevelMap.Values.Sum(r => r.RemainingCount);

        internal EquipmentsGroupByMasterID(EquipmentInfo rpInfo, IEnumerable<Equipment> rpEquipments)
        {
            Info = rpInfo;

            r_LevelMap = rpEquipments.GroupBy(r => new EquipmentGroupingKey(r.Level, r.Proficiency)).ToDictionary(r => r.Key, r => new EquipmentsGroupByLevel(r.Key, r));
            Levels = r_LevelMap.OrderBy(r => r.Key.Level).ThenBy(r => r.Key.Proficiency).Select(r => r.Value).ToArray().AsReadOnly();
        }

        internal void Update(Ship rpShip, EquipmentGroupingKey rpLevel) => r_LevelMap[rpLevel].Update(rpShip);
    }
}
