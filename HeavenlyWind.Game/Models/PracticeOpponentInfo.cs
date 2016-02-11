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
        public string Comment => RawData.Comment;

        public int Level => RawData.Level;
        public AdmiralRank Rank => RawData.Rank;

        public string FleetName => RawData.FleetName;
        public IList<IParticipant> Ships { get; private set; }

        public int Experience { get; private set; }
        public int? ExperienceBonus { get; private set; }

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

            CalcultateExperienceBonusFromCLP();
        }

        void CalcultateExperienceBonusFromCLP()
        {
            var rFleet = KanColleGame.Current.Port.Fleets[1];
            var rCLPs = rFleet.Ships.Where(r => r.Info.Type.ID == 21).ToArray();
            if (rCLPs.Length == 0)
                return;

            double rBonus;
            if (rFleet.Ships[0].Info.Type.ID == 21)
            {
                var rLevel = rCLPs[0].Level;

                if (rCLPs.Length == 1)
                {
                    if (rLevel < 10)
                        rBonus = .05;
                    else if (rLevel < 30)
                        rBonus = .08;
                    else if (rLevel < 60)
                        rBonus = .12;
                    else if (rLevel < 100)
                        rBonus = .15;
                    else
                        rBonus = .2;
                }
                else
                {
                    if (rLevel < 10)
                        rBonus = .1;
                    else if (rLevel < 30)
                        rBonus = .13;
                    else if (rLevel < 60)
                        rBonus = .16;
                    else if (rLevel < 100)
                        rBonus = .2;
                    else
                        rBonus = .25;
                }
            }
            else if (rCLPs.Length == 1)
            {
                var rLevel = rCLPs[0].Level;

                if (rLevel < 10)
                    rBonus = .03;
                else if (rLevel < 30)
                    rBonus = .05;
                else if (rLevel < 60)
                    rBonus = .07;
                else if (rLevel < 100)
                    rBonus = .1;
                else
                    rBonus = .15;
            }
            else
            {
                var rLevel = rCLPs.Max(r => r.Level);

                if (rLevel < 10)
                    rBonus = .04;
                else if (rLevel < 30)
                    rBonus = .06;
                else if (rLevel < 60)
                    rBonus = .08;
                else if (rLevel < 100)
                    rBonus = .12;
                else
                    rBonus = .175;
            }

            ExperienceBonus = (int)(Experience * rBonus);

            OnPropertyChanged(nameof(ExperienceBonus));
        }
    }
}
