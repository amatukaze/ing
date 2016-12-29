using Sakuno.KanColle.Amatsukaze.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetLoSStatus : ModelBase
    {
        Fleet r_Fleet;
        FleetLoSFormulaInfo r_Formula;

        public FleetLoSFormula Formula => r_Formula.Name;

        public double LoS { get; private set; }

        internal FleetLoSStatus(Fleet rpFleet, FleetLoSFormulaInfo rpFormula)
        {
            r_Fleet = rpFleet;
            r_Formula = rpFormula;
        }

        internal void Update()
        {
            LoS = r_Formula.Calculate(r_Fleet);
            OnPropertyChanged(nameof(LoS));
        }
    }
}
