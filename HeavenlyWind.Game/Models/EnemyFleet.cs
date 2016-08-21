using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EnemyFleet : ModelBase
    {
        public IList<ShipInfo> Ships { get; }

        public List<Formation> Formations { get; } = new List<Formation>();

        internal EnemyFleet(IEnumerable<int> rpShips)
        {
            Ships = rpShips.Select(r => KanColleGame.Current.MasterInfo.Ships[r]).ToArray();
        }
    }
}
