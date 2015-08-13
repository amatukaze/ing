using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        Headquarter r_Headquarter;
        public Headquarter Headquarter
        {
            get { return r_Headquarter; }
            set
            {
                if (r_Headquarter != value)
                {
                    r_Headquarter = value;
                    OnPropertyChanged(nameof(Headquarter));
                }
            }
        }

        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        public IDTable<Fleet> Fleets { get; } = new IDTable<Fleet>();

        public IDTable<Equipment> Equipments { get; } = new IDTable<Equipment>();

        internal Port()
        {
        }

        internal void UpdateHeadquarter(RawBasic rpData)
        {
            if (Headquarter == null)
                Headquarter = new Headquarter(rpData);
            else
                Headquarter.Update(rpData);
        }
    }
}
