using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    class FleetLocking : ModelBase
    {
        ShipLocking r_Source;

        public int ID => r_Source.ID;

        public string Name => r_Source.Name;

        public string Color => r_Source.Color;

        public IList<string> CanParticipateIn { get; }

        public FleetLocking(ShipLocking rpSource)
        {
            r_Source = rpSource;

            CanParticipateIn = Enumerable.Range(0, KanColleGame.Current.MasterInfo.EventMapCount).Select(r =>
            {
                var rBit = 1 << r;
                return (r_Source.AllowedEventMaps & rBit) == rBit ? Color : null;
            }).ToList();
        }
    }
}
