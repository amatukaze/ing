using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EnemyFleet : ModelBase
    {
        public IList<ShipInfo> MainShips { get; }
        public IList<ShipInfo> EscortShips { get; }

        public List<Formation> Formations { get; } = new List<Formation>();

        internal EnemyFleet(string[] rpShips)
        {
            var rShips = rpShips.Select(int.Parse);

            if (rpShips.Length <= 6)
                MainShips = rShips.Select(r => KanColleGame.Current.MasterInfo.Ships[r]).ToArray();
            else
            {
                MainShips = rShips.Take(6).Where(r => r != -1).Select(r => KanColleGame.Current.MasterInfo.Ships[r]).ToArray();
                EscortShips = rShips.Skip(6).Select(r => KanColleGame.Current.MasterInfo.Ships[r]).ToArray(); ;
            }
        }
    }
}
