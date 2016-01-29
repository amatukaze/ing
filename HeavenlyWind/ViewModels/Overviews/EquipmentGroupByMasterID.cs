using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentGroupByMasterID
    {
        public EquipmentInfo Info { get; }
        public EquipmentTypeViewModel Type { get; }

        Dictionary<EquipmentGroupingKey, EquipmentGroupByLevel> r_LevelMap;
        public IReadOnlyCollection<EquipmentGroupByLevel> Levels { get; }

        public int Count => r_LevelMap.Values.Sum(r => r.Count);
        public int RemainingCount => r_LevelMap.Values.Sum(r => r.RemainingCount);

        internal EquipmentGroupByMasterID(EquipmentInfo rpInfo, EquipmentTypeViewModel rpType, IEnumerable<Equipment> rpEquipment)
        {
            Info = rpInfo;
            Type = rpType;

            r_LevelMap = rpEquipment.GroupBy(r => new EquipmentGroupingKey(r.Level, r.Proficiency)).ToDictionary(r => r.Key, r => new EquipmentGroupByLevel(r.Key, r));
            Levels = r_LevelMap.OrderBy(r => r.Key.Level).ThenBy(r => r.Key.Proficiency).Select(r => r.Value).ToArray().AsReadOnly();
        }

        internal void Update(Ship rpShip, EquipmentGroupingKey rpLevel) => r_LevelMap[rpLevel].Update(rpShip);
    }
}
