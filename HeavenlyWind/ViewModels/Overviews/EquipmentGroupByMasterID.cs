using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentGroupByMasterID : ModelBase
    {
        public EquipmentInfo Info { get; }
        public EquipmentTypeViewModel Type { get; }

        ListDictionary<EquipmentGroupingKey, EquipmentGroupByLevel> r_LevelMap = new ListDictionary<EquipmentGroupingKey, EquipmentGroupByLevel>();
        public IReadOnlyCollection<EquipmentGroupByLevel> Levels { get; }

        public int Count => r_LevelMap.Values.Sum(r => r.Count);
        public int RemainingCount => r_LevelMap.Values.Sum(r => r.RemainingCount);

        internal EquipmentGroupByMasterID(EquipmentInfo rpInfo, EquipmentTypeViewModel rpType, IEnumerable<Equipment> rpEquipment)
        {
            Info = rpInfo;
            Type = rpType;

            foreach (var rGroup in rpEquipment.GroupBy(r => new EquipmentGroupingKey(r.Level, r.Proficiency)))
                r_LevelMap.Add(rGroup.Key, new EquipmentGroupByLevel(rGroup.Key, rGroup));
            Levels = r_LevelMap.OrderBy(r => r.Key.Level).ThenBy(r => r.Key.Proficiency).Select(r => r.Value).ToArray().AsReadOnly();
        }

        internal void Update(Ship rpShip, EquipmentGroupingKey rpLevel) => r_LevelMap[rpLevel].Update(rpShip);
    }
}
