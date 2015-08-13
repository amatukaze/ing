using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public struct ClampedValue : IEquatable<ClampedValue>
    {
        public int Current { get; }
        public int Maximum { get; }

        public ClampedValue(int rpMaximum, int rpCurrent)
        {
            Maximum = rpMaximum;
            Current = rpCurrent;
        }

        public ClampedValue Update(int rpCurrent) => new ClampedValue(Maximum, rpCurrent);

        public bool Equals(ClampedValue rpOther) => Current == rpOther.Current && Maximum == rpOther.Maximum;
        public override bool Equals(object rpObject) => rpObject != null ? Equals((ClampedValue)rpObject) : false;
        public override int GetHashCode() => Maximum ^ Current;

        public static bool operator ==(ClampedValue x, ClampedValue y) => x.Equals(y);
        public static bool operator !=(ClampedValue x, ClampedValue y) => !x.Equals(y);

        public override string ToString() => $"{Current}/{Maximum}";
    }
}