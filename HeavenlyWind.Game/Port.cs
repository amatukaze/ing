using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        public IDTable<Equipment> Equipments { get; } = new IDTable<Equipment>();

        internal Port()
        {
        }
    }
}
