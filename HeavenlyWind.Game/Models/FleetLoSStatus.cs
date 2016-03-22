using Sakuno.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetLoSStatus : ModelBase
    {
        Fleet r_Fleet;

        public IDictionary<FleetLoSFormulaInfo, double> Formulas { get; private set; }

        internal FleetLoSStatus(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            var rResult = new ListDictionary<FleetLoSFormulaInfo, double>();
            foreach (var rCalculation in FleetLoSFormulaInfo.Formulas.Select(r => new { Formula = r, LoS = r.Calculate(r_Fleet) }))
                rResult.Add(rCalculation.Formula, rCalculation.LoS);

            Formulas = rResult;
            OnPropertyChanged(nameof(Formulas));
        }
    }
}
