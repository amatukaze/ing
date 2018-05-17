namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipModernizationStatus : ModelBase
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public int Delta { get; private set; }

        public int Current => Minimum + Delta;

        public bool IsMaximum => Current == Maximum;

        internal ShipModernizationStatus() { }

        internal void Update(int rpMinimum, int rpMaximum, int rpDelta)
        {
            Minimum = rpMinimum;
            Maximum = rpMaximum;
            Delta = rpDelta;
        }

        public override string ToString() => $"{Current}/{Maximum}";

    }
}
