using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentGroupByMasterID : ModelBase
    {
        public EquipmentInfo Info { get; }
        public FilterTypeViewModel<EquipmentIconType> Type { get; }

        ListDictionary<EquipmentGroupingKey, EquipmentGroupByLevel> r_LevelMap = new ListDictionary<EquipmentGroupingKey, EquipmentGroupByLevel>();
        public IReadOnlyCollection<EquipmentGroupByLevel> Levels { get; }

        public int Count => r_LevelMap.Values.Sum(r => r.Count);
        public int RemainingCount
        {
            get
            {
                var rUnequippedEquipment = KanColleGame.Current.Port.UnequippedEquipment[Info.Type];
                if (rUnequippedEquipment == null)
                    return 0;

                return rUnequippedEquipment.Count(r => r.Info == Info);
            }
        }

        internal EquipmentGroupByMasterID(EquipmentInfo rpInfo, FilterTypeViewModel<EquipmentIconType> rpType, IEnumerable<Equipment> rpEquipment)
        {
            Info = rpInfo;
            Type = rpType;

            foreach (var rGroup in rpEquipment.GroupBy(r => new EquipmentGroupingKey(r.Level, r.Proficiency)))
                r_LevelMap.Add(rGroup.Key, new EquipmentGroupByLevel(this, rGroup.Key, rGroup));
            Levels = r_LevelMap.OrderBy(r => r.Key.Level).ThenBy(r => r.Key.Proficiency).Select(r => r.Value).ToArray().AsReadOnly();
        }

        internal void Update(Ship rpShip, EquipmentGroupingKey rpLevel) => r_LevelMap[rpLevel].Update(rpShip);
    }
}
