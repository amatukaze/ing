using Sakuno.KanColle.Amatsukaze.Game.Models.LoS;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class FleetLoSFormulaInfo
    {
        public static IList<FleetLoSFormulaInfo> Formulas { get; }

        public abstract FleetLoSFormula Name { get; }

        static FleetLoSFormulaInfo()
        {
            Formulas = new FleetLoSFormulaInfo[]
            {
                new OldFormula(),
                new AutumnFormula(),
                new AutumnSimplifiedFormula(),
                new Formula33(FleetLoSFormula.Formula33),
                new Formula33(FleetLoSFormula.Formula33Cn3),
                new Formula33(FleetLoSFormula.Formula33Cn4),
            };
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
