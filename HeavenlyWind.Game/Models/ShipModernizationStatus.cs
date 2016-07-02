namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipModernizationStatus : ModelBase
    {
        public int Minimum { get; internal set; }
        public int Maximum { get; internal set; }

        public int Delta { get; internal set; }
        public int Current => Minimum + Delta;

        public bool IsMaximum => Current == Maximum;

        internal ShipModernizationStatus(int rpMinimum, int rpMaximum, int rpDelta)
        {
            Minimum = rpMinimum;
            Maximum = rpMaximum;

            Delta = rpDelta;
        }

        public override string ToString() => $"{Current}/{Maximum}";
    }
}
