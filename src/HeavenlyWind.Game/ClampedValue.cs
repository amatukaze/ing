using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class ClampedValue : ModelBase, IEquatable<ClampedValue>
    {
        int r_Current;
        public int Current
        {
            get { return r_Current; }
            set
            {
                if (r_Current != value)
                {
                    r_Current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        int r_Before;
        public int Before
        {
            get { return r_Before; }
            set
            {
                if (r_Before != value)
                {
                    r_Before = value;
                    OnPropertyChanged(nameof(Before));
                }
            }
        }

        int r_Maximum;
        public int Maximum
        {
            get { return r_Maximum; }
            set
            {
                if (r_Maximum != value)
                {
                    r_Maximum = value;
                    OnPropertyChanged(nameof(Maximum));
                }
            }
        }

        public ClampedValue(int rpMaximum, int rpCurrent) : this(rpMaximum, rpCurrent, rpCurrent) { }
        public ClampedValue(int rpMaximum, int rpBefore, int rpCurrent)
        {
            Maximum = rpMaximum;
            Before = rpBefore;
            Current = rpCurrent;
        }

        public virtual void Set(int rpMaximum, int rpCurrent)
        {
            Maximum = rpMaximum;
            Current = rpCurrent;
        }

        public bool Equals(ClampedValue rpOther) => rpOther == this;
        public override bool Equals(object rpObject)
        {
            var rObject = rpObject as ClampedValue;
            if (rObject == null)
                return false;

            return Equals(rObject);
        }
        public override int GetHashCode() => (Maximum * 401) ^ (Before * 19) ^ Current;

        public static bool operator ==(ClampedValue x, ClampedValue y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (((object)x) == null || ((object)y) == null)
                return false;

            return x.Current == y.Current && x.Before == y.Before && x.Maximum == y.Maximum;
        }
        public static bool operator !=(ClampedValue x, ClampedValue y) => !(x == y);

        public override string ToString() => $"{(Before == Current ? string.Empty : Before + "->")}{Current}/{Maximum}";
    }
}