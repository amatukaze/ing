using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetLoSStatus : ModelBase
    {
        Fleet r_Fleet;

        public IDictionary<FleetLoSFormula, double> Formulas { get; private set; }

        internal FleetLoSStatus(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            var rResult = new ListDictionary<FleetLoSFormula, double>();
            foreach (var rFormula in FleetLoSFormulaInfo.Formulas)
                rResult.Add(rFormula.Name, rFormula.Calculate(r_Fleet));

            Formulas = rResult;
            OnPropertyChanged(nameof(Formulas));
        }
    }
}
