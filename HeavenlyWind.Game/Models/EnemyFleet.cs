using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EnemyFleet : ModelBase
    {
        public IList<ShipInfo> Ships { get; }

        public Formation Formation { get; }

        internal EnemyFleet(IEnumerable<int> rpShips, Formation rpFormation)
        {
            Ships = rpShips.Select(r => KanColleGame.Current.MasterInfo.Ships[r]).ToList();

            Formation = rpFormation;
        }
    }
}
