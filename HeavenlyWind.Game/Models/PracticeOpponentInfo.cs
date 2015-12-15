using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class PracticeOpponentInfo : RawDataWrapper<RawPracticeOpponentInfo>
    {
        public string Name => RawData.Name;

        public int Level => RawData.Level;
        public AdmiralRank Rank => RawData.Rank;

        public IList<IParticipant> Ships { get; private set; }

        public int Experience { get; private set; }

        internal PracticeOpponentInfo(RawPracticeOpponentInfo rpRawData) : base(rpRawData)
        {
            UpdateShips();
        }

        protected override void OnRawDataUpdated()
        {
            UpdateShips();

            OnPropertyChanged(nameof(Name));
        }

        void UpdateShips()
        {
            Ships = RawData.Fleet.Ships.TakeWhile(r => r.ID != -1).Select(r => new EnemyShip(r.ShipID, r.Level)).ToList<IParticipant>();

            var rShips = RawData.Fleet.Ships;
            var rLevel = rShips[0].ID != -1 ? rShips[0].Level : 1;
            var rLevel2 = rShips[1].ID != -1 ? rShips[1].Level : 1;
            var rExperience = ExperienceTable.Ship[rLevel].Total / 100.0 + ExperienceTable.Ship[rLevel2].Total / 300.0;
            if (rExperience >= 500.0)
                rExperience = 500.0 + Math.Sqrt(rExperience - 500.0);

            Experience = (int)rExperience;

            OnPropertyChanged(nameof(Ships));
            OnPropertyChanged(nameof(Experience));
        }
    }
}
