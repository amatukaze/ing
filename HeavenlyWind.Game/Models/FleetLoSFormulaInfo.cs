using Sakuno.KanColle.Amatsukaze.Game.Models.LoS;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class FleetLoSFormulaInfo
    {
        static List<FleetLoSFormulaInfo> r_Formulas;
        public static IList<FleetLoSFormulaInfo> Formulas { get; }

        public abstract FleetLoSFormula Name { get; }

        static FleetLoSFormulaInfo()
        {
            r_Formulas = new List<FleetLoSFormulaInfo>()
            {
                new OldFormula(),
                new AutumnFormula(),
                new AutumnSimplifiedFormula(),
                new Formula33(),
            };
            Formulas = r_Formulas.AsReadOnly();
        }

        public double Calculate(Fleet rpFleet)
        {
            if (rpFleet == null || rpFleet.Ships.Count == 0)
                return 0;
            else
                return CalculateCore(rpFleet);
        }
        protected abstract double CalculateCore(Fleet rpFleet);
    }
}
