using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public struct ClampedValue : IEquatable<ClampedValue>
    {
        public int Current { get; }
        public int Before { get; set; }
        public int Maximum { get; }

        public ClampedValue(int rpMaximum, int rpCurrent) : this(rpMaximum, rpCurrent, rpCurrent) { }
        public ClampedValue(int rpMaximum, int rpBefore, int rpCurrent)
        {
            Maximum = rpMaximum;
            Before = rpBefore;
            Current = rpCurrent;
        }

        public ClampedValue Update(int rpCurrent) => new ClampedValue(Maximum, rpCurrent);

        public bool Equals(ClampedValue rpOther) => Current == rpOther.Current && Before == rpOther.Before && Maximum == rpOther.Maximum;
        public override bool Equals(object rpObject) => rpObject != null ? Equals((ClampedValue)rpObject) : false;
        public override int GetHashCode() => (Maximum * 401) ^ (Before * 401) ^ Current;

        public static bool operator ==(ClampedValue x, ClampedValue y) => x.Equals(y);
        public static bool operator !=(ClampedValue x, ClampedValue y) => !x.Equals(y);

        public override string ToString() => $"{(Before == Current ? string.Empty : Before + "->")}{Current}/{Maximum}";
    }
}